#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ICSharpCode.SharpZipLib.Zip;
using Nito.AsyncEx;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Tools;
using Timer = System.Timers.Timer;
#pragma warning disable 4014

namespace PlcNext.NamedPipeServer.Communication
{
    public class NamedPipeCommunicationProtocol : ICommunicationProtocol
    {
        private readonly PipeStream readStream;
        private readonly PipeStream writeStream;
        private readonly StreamFactory streamFactory;
        private readonly ILog log;
        public const int HeaderSize = 21;
        public const int MaxMessageLength = int.MaxValue - HeaderSize;
        public const int SplitMessageIndicator = int.MinValue;
        public const byte DefaultConfirmationFlag = 0x00;
        public const byte SuccessConfirmationFlag = 0x01;
        public const byte ErrorConfirmationFlag = 0xFF;
        public const int MaxRetrySendingCount = 3;
        public const int MaxConfirmationResponseTime = 100;
        public const int BufferSize = 4096;
        private bool disconnected;
        private readonly object disconnectedLock = new object();
        private OutgoingMessageQueue outgoingMessageQueue;
        private IncomingMessageQueue incomingMessageQueue;

        private readonly CancellationTokenSource disposedCancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken CancellationToken => disposedCancellationTokenSource.Token;

        protected NamedPipeCommunicationProtocol(PipeStream serverStream, StreamFactory streamFactory, ILog log) : this(
            serverStream, serverStream, streamFactory, log)
        {
        }

        private NamedPipeCommunicationProtocol(PipeStream readStream, PipeStream writeStream, StreamFactory streamFactory, ILog log)
        {
            this.readStream = readStream;
            this.writeStream = writeStream;
            this.streamFactory = streamFactory;
            this.log = log;
        }

        public void Start()
        {
            outgoingMessageQueue = CreateOutgoingMessageQueue();
            outgoingMessageQueue.WaitForFirstMessage();
            incomingMessageQueue = CreateIncomingMessageQueue(outgoingMessageQueue);
            incomingMessageQueue.MessageReceived += OnMessageReceived;
            incomingMessageQueue.WaitForFirstMessage();
        }

        public void FlushReceivedMessages()
        {
            log.LogInformation("Flush all received messages explicitly.");
            incomingMessageQueue.Flush();
        }

        protected virtual OutgoingMessageQueue CreateOutgoingMessageQueue()
        {
            return new OutgoingMessageQueue(log, CancellationToken, writeStream);
        }

        protected virtual IncomingMessageQueue CreateIncomingMessageQueue(OutgoingMessageQueue outgoingQueue)
        {
            return new IncomingMessageQueue(streamFactory, log, readStream,
                                            this, CancellationToken, outgoingQueue);
        }

        protected void Disconnect()
        {
            lock (disconnectedLock)
            {
                if (disconnected)
                {
                    return;
                }

                disconnected = true;
            }

            FlushReceivedMessages();
            
            log.LogInformation("Disconnecting the pipe stream.");
            disposedCancellationTokenSource.Cancel();
            
            if (incomingMessageQueue != null)
            {
                incomingMessageQueue.MessageReceived -= OnMessageReceived;
                incomingMessageQueue.Dispose();
            }
            outgoingMessageQueue?.Dispose();
            
            if (readStream.IsConnected && readStream is NamedPipeServerStream serverReadStream)
            {
                serverReadStream.Disconnect();
            }
            readStream.Close();
            readStream.Dispose();
            if (writeStream.IsConnected && writeStream is NamedPipeServerStream serverWriteStream)
            {
                serverWriteStream.Disconnect();
            }
            writeStream.Close();
            writeStream.Dispose();
            OnError();
        }

        public static async Task<ICommunicationProtocol> Connect(string address, StreamFactory streamFactory, 
                                                                ILog log,
                                                                bool twoChannelCommunication = false, 
                                                                CancellationToken cancellationToken = default(CancellationToken),
                                                                bool actAsClient = false)
        {
            NamedPipeCommunicationProtocol result;
            if (twoChannelCommunication && !actAsClient)
            {
                NamedPipeServerStream writeServer = new NamedPipeServerStream(Path.Combine(address, "server-output"), PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.Asynchronous);
                NamedPipeServerStream readServer = new NamedPipeServerStream(Path.Combine(address, "server-input"), PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.Asynchronous);
                await readServer.WaitForConnectionAsync(cancellationToken);
                await writeServer.WaitForConnectionAsync(cancellationToken);

                result = new NamedPipeCommunicationProtocol(readServer, writeServer, streamFactory, new ServerNameLogDecorator(log,address,false));
            }
            else if(!twoChannelCommunication && !actAsClient)
            {
                NamedPipeServerStream server = new NamedPipeServerStream(address, PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync(cancellationToken);

                result = new NamedPipeCommunicationProtocol(server, streamFactory, new ServerNameLogDecorator(log,address,false));
            }
            else if(twoChannelCommunication)
            {
                NamedPipeClientStream writeClient = new NamedPipeClientStream(".", Path.Combine(address, "server-input"), PipeDirection.In,
                                                                             PipeOptions.Asynchronous);
                NamedPipeClientStream readClient = new NamedPipeClientStream(".", Path.Combine(address, "server-input"), PipeDirection.Out,
                                                                              PipeOptions.Asynchronous);
                await readClient.ConnectAsync(cancellationToken);
                await writeClient.ConnectAsync(cancellationToken);

                result = new NamedPipeCommunicationProtocol(readClient, writeClient, streamFactory, new ServerNameLogDecorator(log,address,true));
            }
            else
            {
                NamedPipeClientStream client = new NamedPipeClientStream(".", address, PipeDirection.InOut,
                                                                             PipeOptions.Asynchronous);
                await client.ConnectAsync(cancellationToken);

                result = new NamedPipeCommunicationProtocol(client, streamFactory, new ServerNameLogDecorator(log,address,true));
            }
            return result;
        }

        public void SendMessage(Stream message, Action messageCompletedAction = null)
        {
            Guid messageGuid = Guid.NewGuid();

            Stream messageCopy = streamFactory.Create(message.Length);
            message.CopyTo(messageCopy, BufferSize);

            OutgoingMessage outgoingMessage = GenerateOutgoingMessage(messageGuid, messageCopy, messageCompletedAction);
            outgoingMessageQueue.Enqueue(outgoingMessage);
        }

        protected virtual OutgoingMessage GenerateOutgoingMessage(Guid messageGuid, Stream messageCopy,
                                                                  Action messageCompletedAction)
        {
            return new OutgoingMessage(messageGuid, messageCopy, messageCompletedAction, this, log, writeStream);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public virtual void Dispose()
        {
            Disconnect();
        }

        public event EventHandler<EventArgs> Error;

        protected virtual void OnError()
        {
            Error?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            MessageReceived?.Invoke(this, messageReceivedEventArgs);
        }
        
        protected class IncomingMessageQueue : IDisposable
        {
            private readonly StreamFactory streamFactory;
            private readonly ILog log;
            private readonly PipeStream readStream;
            private readonly NamedPipeCommunicationProtocol communicationProtocol;
            private readonly ConcurrentQueue<IncomingMessage> messages = new ConcurrentQueue<IncomingMessage>();
            private readonly ConcurrentDictionary<Guid, IncomingMessage> splitMessages = new ConcurrentDictionary<Guid, IncomingMessage>();
            private readonly CancellationToken cancellationToken;
            private readonly OutgoingMessageQueue outgoingMessageQueue;
            private readonly object flushLock = new object();
            
            private Thread readingThread;
            private PollingCollectionObserver pollingCollectionObserver;
            
            public IncomingMessageQueue(StreamFactory streamFactory, ILog log, PipeStream readStream, NamedPipeCommunicationProtocol communicationProtocol, CancellationToken cancellationToken, OutgoingMessageQueue outgoingMessageQueue)
            {
                this.streamFactory = streamFactory;
                this.log = log;
                this.readStream = readStream;
                this.communicationProtocol = communicationProtocol;
                this.cancellationToken = cancellationToken;
                this.outgoingMessageQueue = outgoingMessageQueue;
            }

            public event EventHandler<MessageReceivedEventArgs> MessageReceived;

            public void WaitForFirstMessage()
            {
                readingThread = new Thread(StartReading)
                {
                    Priority = ThreadPriority.AboveNormal,
                    IsBackground = true
                };
                readingThread.Start();
                pollingCollectionObserver = PollingCollectionObserver.Observe(Flush, messages);
            }

            public void Flush()
            {
                lock (flushLock)
                {
                    while (messages.TryDequeue(out IncomingMessage current))
                    {
                        OnMessageReceived(new MessageReceivedEventArgs(current.Data));
                        current.Dispose();
                    }
                }
            }

            private async void StartReading()
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        log.LogInformation("Start message reader");
                        byte[] header = new byte[HeaderSize];
                        int headerResult = await readStream.ReadAsync(header, 0, header.Length, cancellationToken);
                        try
                        {
                            if (headerResult == 0)
                            {
                                throw new ClientDisconnectedException();
                            }

                            (int messageLength, bool isSplit, Guid messageGuid, byte confirmation) = ParseHeader(header);

                            if (messageLength > 0)
                            {
                                await ReadMessage(messageLength, isSplit, messageGuid, confirmation);
                            }
                            else
                            {
                                ProcessConfirmationFlag(confirmation, messageGuid);
                            }
                        }
                        catch (ClientDisconnectedException)
                        {
                            communicationProtocol.Disconnect();
                        }
                    }
                    log.LogInformation("Shutdown reader thread.");
                }
                catch (Exception)
                {
                    //Do not log anything as any log will lead to another exception
                }

                async Task ReadMessage(int messageLength, bool isSplit, Guid messageGuid, byte confirmation)
                {
                    Stream messageStream = streamFactory.Create(messageLength);
                    int intervals = messageLength / BufferSize;
                    int remaining = messageLength % BufferSize;
                    byte[] buffer = new byte[BufferSize];
                    IncomingMessage message = new IncomingMessage(messageGuid, messageStream, log);

                    try
                    {
                        await ReadIntervals();
                        CancellationToken delayToken = new CancellationTokenSource(MaxConfirmationResponseTime).Token;
                        int result = await readStream.ReadAsync(buffer, 0, remaining,
                                                                CancellationTokenSource.CreateLinkedTokenSource(delayToken, 
                                                                                                                cancellationToken)
                                                                                       .Token);

                        if (result == 0)
                        {
                            throw new ClientDisconnectedException();
                        }

                        if (result != remaining)
                        {
                            throw new PartialMessageException(messageGuid);
                        }

                        await AppendBufferAsync(messageStream, buffer, remaining);
                        messageStream.Seek(0, SeekOrigin.Begin);

                        if (splitMessages.ContainsKey(messageGuid) &&
                            splitMessages.TryGetValue(messageGuid, out IncomingMessage splitMessage))
                        {
                            await MergeWithSplitMessage(splitMessage, messageStream);
                            message = splitMessage;
                        }
                        
                        if (isSplit)
                        {
                            EnqueueSplitMessage(messageGuid, message);
                        }

                        if (!isSplit)
                        {
                            splitMessages.TryRemove(messageGuid, out _);

                            CompleteMessage(message, messageGuid, confirmation);
                        }
                    }
                    catch (ClientDisconnectedException disconnectedException)
                    {
                        log.LogError($"Client disconnected during communication.{Environment.NewLine}" +
                                     $"{disconnectedException}");
                        messageStream.Dispose();
                        throw;
                    }
                    catch (Exception e)
                    {
                        switch (e)
                        {
                            case TaskCanceledException c:
                                e = new MessageTimeoutException(messageGuid, MaxConfirmationResponseTime, c);
                                break;
                            case OperationCanceledException oc:
                                e = new MessageTimeoutException(messageGuid, MaxConfirmationResponseTime, oc);
                                break;
                        }

                        log.LogError($"Exception during message read.{Environment.NewLine}" +
                                     $"{e}");
                        outgoingMessageQueue.SendMessageConfirmation(messageGuid, false);
                        message.Dispose();
                        messageStream.Dispose();
                    }
                    
                    log.LogVerbose("Finished reading message.");

                    async Task ReadIntervals()
                    {
                        while (intervals > 0)
                        {
                            intervals--;
                            await ReadInterval();
                        }

                        async Task ReadInterval()
                        {
                            CancellationToken delayToken = new CancellationTokenSource(MaxConfirmationResponseTime).Token;
                            int result = await readStream.ReadAsync(buffer, 0, BufferSize,
                                                                    CancellationTokenSource.CreateLinkedTokenSource(delayToken, 
                                                                                                                    cancellationToken)
                                                                       .Token);

                            if (result == 0)
                            {
                                throw new ClientDisconnectedException();
                            }

                            if (result != BufferSize)
                            {
                                throw new PartialMessageException(messageGuid);
                            }

                            await AppendBufferAsync(messageStream, buffer, BufferSize);
                        }
                    }
                }
            }

            protected virtual void ProcessConfirmationFlag(byte confirmation, Guid messageGuid)
            {
                switch (confirmation)
                {
                    case SuccessConfirmationFlag:
                        outgoingMessageQueue.ConfirmAndContinue(messageGuid);
                        break;
                    case ErrorConfirmationFlag:
                        outgoingMessageQueue.ResendMessage(messageGuid);
                        break;
                    default:
                        log.LogError($"The message {messageGuid.ToByteString()} was send without content and had the confirmation flag 0x{confirmation:X}." + "The protocol does not understand this message.");
                        break;
                }
            }

            protected virtual void CompleteMessage(IncomingMessage message, Guid messageGuid, byte confirmation)
            {
                if (confirmation == DefaultConfirmationFlag)
                {
                    LogConcreteMessage();
                    message.Data.Seek(0, SeekOrigin.Begin);
                    messages.Enqueue(message);
                    log.LogInformation($"Received message {messageGuid.ToByteString()} with {message.Data.Length} bytes.");
                    outgoingMessageQueue.SendMessageConfirmation(messageGuid, true);
                }
                else
                {
                    log.LogError(
                        $"Message {messageGuid.ToByteString()} was send with body and confirmation flag 0x{confirmation:X}. The message body will be ignored");
                }
                
                void LogConcreteMessage()
                {
                    try
                    {
                        if (message.Data.Length < 1024)
                        {
                            message.Data.Seek(0, SeekOrigin.Begin);
                            //log message text
                            using (StreamReader reader = new StreamReader(message.Data, Encoding.UTF8,
                                                                          true, 1024,
                                                                          true))
                            {
                                log.LogVerbose(
                                    $"Content of received message is{Environment.NewLine}{reader.ReadToEnd()}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.LogVerbose($"Error while trying to log received message:{Environment.NewLine}{e}");
                    }
                }
            }

            protected virtual async Task MergeWithSplitMessage(IncomingMessage splitMessage, Stream messageStream)
            {
                await splitMessage.AppendMessageStreamAsync(messageStream, cancellationToken);
                messageStream.Dispose();
            }

            protected virtual void EnqueueSplitMessage(Guid messageGuid, IncomingMessage message)
            {
                splitMessages.TryAdd(messageGuid, message);
            }

            protected virtual async Task AppendBufferAsync(Stream messageStream, byte[] buffer, int length)
            {
                //TODO this blocks when writing for the first time in SendSplitMessage Test
                await messageStream.WriteAsync(buffer, 0, length, cancellationToken);
            }

            protected virtual (int messageLength, bool isSplit, Guid messageGuid, byte confirmation) ParseHeader(byte[] header)
            {
                int messageLength = BitConverter.ToInt32(header.Take(4).ToArray().BigEndian(), 0);
                bool isSplit = false;
                if (messageLength == SplitMessageIndicator)
                {
                    messageLength = MaxMessageLength;
                    isSplit = true;
                }

                Guid messageGuid = new Guid(header.Skip(sizeof(int)).Take(16).ToArray());
                byte confirmation = header[HeaderSize - 1];
                log.LogInformation(
                    $"Received message header for message {messageGuid.ToByteString()} length: {messageLength} confirmation: 0x{confirmation:X}");
                return (messageLength, isSplit, messageGuid, confirmation);
            }

            public void Dispose()
            {
                readingThread?.Join();
                pollingCollectionObserver?.Dispose();
                
                foreach (IncomingMessage message in messages)
                {
                    message.Dispose();
                }

                foreach (IncomingMessage splitMessage in splitMessages.Values)
                {
                    splitMessage.Dispose();
                }
            }

            private void OnMessageReceived(MessageReceivedEventArgs e)
            {
                MessageReceived?.Invoke(this, e);
            }
        }

        protected class ServerNameLogDecorator : ILog
        {
            private readonly ILog logImplementation;
            private readonly string serverName;
            private readonly bool isClient;

            public ServerNameLogDecorator(ILog logImplementation, string serverName, bool isClient)
            {
                this.logImplementation = logImplementation;
                this.serverName = serverName;
                this.isClient = isClient;
            }

            public void LogVerbose(string message)
            {
                logImplementation.LogVerbose($"{GetPrefix()}: {message}");
            }

            public void LogInformation(string message)
            {
                logImplementation.LogInformation($"{GetPrefix()}: {message}");
            }

            public void LogWarning(string message)
            {
                logImplementation.LogWarning($"{GetPrefix()}: {message}");
            }

            public void LogError(string message)
            {
                logImplementation.LogError($"{GetPrefix()}: {message}");
            }

            protected virtual string GetPrefix()
            {
                return (isClient ? $"client-{serverName}" : serverName);
            }
        }

        protected class OutgoingMessageQueue : IDisposable
        {
            private readonly ConcurrentQueue<OutgoingMessage> pendingMessages = new ConcurrentQueue<OutgoingMessage>();
            private readonly ConcurrentQueue<Confirmation> pendingConfirmations = new ConcurrentQueue<Confirmation>();
            private readonly ConcurrentQueue<OutgoingMessage> resendMessages = new ConcurrentQueue<OutgoingMessage>();
            private readonly ConcurrentDictionary<Guid, OutgoingMessage> unconfirmedMessages = new ConcurrentDictionary<Guid, OutgoingMessage>();
            private readonly ILog log;
            private readonly CancellationToken cancellationToken;
            private readonly PipeStream writeStream;
            private readonly Timer confirmationTimer = new Timer(MaxConfirmationResponseTime) {AutoReset = false};
            
            private PollingCollectionObserver pollingCollectionObserver;
            private int sending;

            public OutgoingMessageQueue(ILog log, CancellationToken cancellationToken, PipeStream writeStream)
            {
                this.log = log;
                this.cancellationToken = cancellationToken;
                this.writeStream = writeStream;
                confirmationTimer.Elapsed += ConfirmationTimerOnElapsed;
            }

            private void ConfirmationTimerOnElapsed(object sender, ElapsedEventArgs e)
            {
                if (unconfirmedMessages.TryRemove(unconfirmedMessages.Keys.FirstOrDefault(),
                                                  out OutgoingMessage message))
                {
                    ResendMessage(message);
                }
            }

            public void WaitForFirstMessage()
            {
                pollingCollectionObserver = PollingCollectionObserver.Observe(async () =>
                {
                    try
                    {
                        if (Interlocked.Exchange(ref sending, 1) == 1)
                        {
                            return;
                        }
                    
                        while (pendingConfirmations.TryDequeue(out Confirmation confirmation))
                        {
                            await confirmation.SendAsync(cancellationToken);
                        }

                        if (resendMessages.TryDequeue(out OutgoingMessage resend))
                        {
                            do
                            {
                                await SendMessage(resend);
                            } while (resendMessages.TryDequeue(out resend));
                        }
                        else if (unconfirmedMessages.IsEmpty && pendingMessages.TryDequeue(out OutgoingMessage message))
                        {
                            await SendMessage(message);
                        }

                        sending = 0;
                    }
                    catch (Exception)
                    {
                        //do nothing as this is executed without owning task
                    }
                }, pendingConfirmations, pendingMessages, resendMessages);
                
                async Task SendMessage(OutgoingMessage message)
                {
                    unconfirmedMessages.TryAdd(message.Id, message);
                    await message.SendMessageAsync(cancellationToken);
                    confirmationTimer.Start();
                }
            }

            public void ConfirmAndContinue(Guid messageId)
            {
                if (unconfirmedMessages.TryRemove(messageId, out _))
                {
                    confirmationTimer.Stop();
                    log.LogInformation($"Confirmed message {messageId.ToByteString()}.");
                }
                else
                {
                    log.LogError($"Message {messageId.ToByteString()} cannot be confirmed. There is no such unconfirmed message.");
                }
            }

            public void ResendMessage(Guid messageId)
            {
                if (unconfirmedMessages.TryRemove(messageId, out OutgoingMessage message))
                {
                    ResendMessage(message);
                }
                else
                {
                    log.LogError($"Message {messageId.ToByteString()} cannot be resend. There is no such unconfirmed message.");
                }
            }

            private void ResendMessage(OutgoingMessage message)
            {
                confirmationTimer.Stop();
                log.LogVerbose($"Resend message {message.Id.ToByteString()} explicitly.");
                resendMessages.Enqueue(message);
            }

            public void Enqueue(OutgoingMessage outgoingMessage)
            {
                log.LogVerbose($"Enqueue message {outgoingMessage.Id.ToByteString()} with length {outgoingMessage.Data.Length}.");
                pendingMessages.Enqueue(outgoingMessage);
            }

            public void SendMessageConfirmation(Guid messageId, bool success)
            {
                log.LogVerbose($"Enqueue message confirmation {messageId.ToByteString()}.");
                pendingConfirmations.Enqueue(CreateConfirmation(messageId, success));
            }

            protected virtual Confirmation CreateConfirmation(Guid messageId, bool success)
            {
                return new Confirmation(messageId, success, writeStream, log);
            }

            public void Dispose()
            {
                confirmationTimer.Elapsed -= ConfirmationTimerOnElapsed;
                confirmationTimer.Dispose();
                
                pollingCollectionObserver?.Dispose();
                
                foreach (OutgoingMessage message in pendingMessages)
                {
                    message.Dispose();
                }

                foreach (OutgoingMessage outgoingMessage in unconfirmedMessages.Values.ToArray())
                {
                    outgoingMessage.Dispose();
                }

                foreach (OutgoingMessage resendMessage in resendMessages)
                {
                    resendMessage.Dispose();
                }
            }
        }

        protected class Confirmation
        {
            private readonly Guid messageId;
            private readonly bool success;
            private readonly PipeStream writeStream;
            private readonly ILog log;

            public Confirmation(Guid messageId, bool success, PipeStream writeStream, ILog log)
            {
                this.messageId = messageId;
                this.success = success;
                this.writeStream = writeStream;
                this.log = log;
            }

            public virtual async Task SendAsync(CancellationToken cancellationToken)
            {
                byte[] confirmationHeader = GenerateHeader();
                    
                log.LogInformation($"Send message confirmation for message {messageId.ToByteString()}.");
                await writeStream.WriteAsync(confirmationHeader, 0, confirmationHeader.Length, cancellationToken);
            }

            protected virtual byte[] GenerateHeader()
            {
                return BitConverter.GetBytes(0)
                                   .BigEndian()
                                   .Concat(messageId.ToByteArray())
                                   .Concat(new []
                                    {
                                        success
                                            ? SuccessConfirmationFlag
                                            : ErrorConfirmationFlag
                                    })
                                   .ToArray();
            }
        }

        protected class OutgoingMessage : Message
        {
            private readonly Action messageCompletedAction;
            private readonly NamedPipeCommunicationProtocol communicationProtocol;
            private int sendCounter;
            private readonly PipeStream writeStream;

            public OutgoingMessage(Guid id, Stream data, Action messageCompletedAction,
                                   NamedPipeCommunicationProtocol communicationProtocol, ILog log,
                                   PipeStream writeStream) : base(id, data, log)
            {
                this.messageCompletedAction = messageCompletedAction;
                this.communicationProtocol = communicationProtocol;
                this.writeStream = writeStream;
            }

            private bool HasUnsentData => Data.Position < Data.Length;

            protected bool NextMessageIsSplit => Data.Length - Data.Position > MaxMessageLength;

            protected int RemainingLength => (int) (Data.Length - Data.Position);

            public async Task SendMessageAsync(CancellationToken cancellationToken)
            {
                sendCounter++;
                if (sendCounter > MaxRetrySendingCount)
                {
                    communicationProtocol.Disconnect();
                    return;
                }

                Log.LogInformation($"Start sending message {Id.ToByteString()}. Attempt nr. {sendCounter}.");

                Data.Seek(0, SeekOrigin.Begin);
                await SendMessageInChunks(cancellationToken);
            }

            protected virtual async Task SendMessageInChunks(CancellationToken cancellationToken)
            {
                while (HasUnsentData)
                {
                    byte[] header = GenerateMessageHeader(Id,
                                                          NextMessageIsSplit
                                                              ? SplitMessageIndicator
                                                              : RemainingLength);
                    await writeStream.WriteAsync(header, 0, header.Length, cancellationToken);

                    int sentDataLength = NextMessageIsSplit
                                             ? MaxMessageLength
                                             : RemainingLength;
                    Log.LogVerbose($"Sending data with size {sentDataLength}.");
                    int chunks = sentDataLength / BufferSize;
                    int remaining = sentDataLength % BufferSize;
                    for (int i = 0; i < chunks; i++)
                    {
                        byte[] buffer = new byte[BufferSize];
                        await Data.ReadAsync(buffer, 0, BufferSize, cancellationToken);
                        await writeStream.WriteAsync(buffer, 0, BufferSize, cancellationToken);
                    }

                    byte[] remainingBuffer = new byte[remaining];
                    await Data.ReadAsync(remainingBuffer, 0, remaining, cancellationToken);
                    await writeStream.WriteAsync(remainingBuffer, 0, remaining, cancellationToken);
                    Log.LogInformation($"Message {Id.ToByteString()} send.");
                }
                messageCompletedAction?.Invoke();
            }

            protected virtual byte[] GenerateMessageHeader(Guid guid, int length)
            {
                Log.LogVerbose($"Sending header with size {length}.");
                return BitConverter.GetBytes(length)
                                   .BigEndian()
                                   .Concat(guid.ToByteArray())
                                   .Concat(new[] { DefaultConfirmationFlag })
                                   .ToArray();
            }
        }

        protected class IncomingMessage : Message
        {
            public IncomingMessage(Guid id, Stream data, ILog log) : base(id, data, log)
            {
            }

            public async Task AppendMessageStreamAsync(Stream messageStream, CancellationToken token)
            {
                int intervals = (int)messageStream.Length / BufferSize;
                int remaining = (int)messageStream.Length % BufferSize;
                byte[] buffer = new byte[BufferSize];

                Data.Seek(0, SeekOrigin.End);
                Data.SetLength(Data.Length + messageStream.Length);
                messageStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < intervals; i++)
                {
                    await messageStream.ReadAsync(buffer, 0, BufferSize, token);
                    await Data.WriteAsync(buffer, 0, BufferSize, token);
                }
                await messageStream.ReadAsync(buffer, 0, remaining, token);
                await Data.WriteAsync(buffer, 0, remaining, token);
            }
        }

        protected abstract class Message : IDisposable
        {
            protected readonly ILog Log;

            protected Message(Guid id, Stream data, ILog log)
            {
                Log = log;
                Id = id;
                Data = data;
            }

            public Stream Data { get; }
            public Guid Id { get; }

            public virtual void Dispose()
            {
                Data?.Dispose();
            }
        }
    }
}
