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
        public const int MaxConfirmationResponseTime = 200;
        public const int BufferSize = 4096;
        private bool disconnected;
        private readonly object disconnectedLock = new object();
        private OutgoingMessageQueue outgoingMessageQueue;
        private IncomingMessageQueue incomingMessageQueue;

        private readonly CancellationTokenSource disposedCancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken CancellationToken => disposedCancellationTokenSource.Token;

        protected NamedPipeCommunicationProtocol(PipeStream readStream, PipeStream writeStream, StreamFactory streamFactory, ILog log)
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
            return new OutgoingMessageQueue(log, writeStream, this, CancellationToken);
        }

        protected virtual IncomingMessageQueue CreateIncomingMessageQueue(OutgoingMessageQueue outgoingQueue)
        {
            return new IncomingMessageQueue(streamFactory, log, readStream,
                                            this, outgoingQueue, CancellationToken);
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
            
            //first disconnect write stream to free other reader and signal shutdown
            writeStream.Close();
            writeStream.Dispose();
            
            //next dispose reader
            readStream.Close();
            readStream.Dispose();
            
            //lastly wait for threads to close on their own
            if (incomingMessageQueue != null)
            {
                incomingMessageQueue.MessageReceived -= OnMessageReceived;
                incomingMessageQueue.Dispose();
            }
            outgoingMessageQueue?.Dispose();
            disposedCancellationTokenSource.Dispose();

            //and report disconnect
            OnError();
        }

        public static async Task<ICommunicationProtocol> Connect(string address, StreamFactory streamFactory,
                                                                 ILog log,
                                                                 bool actAsClient = false,
                                                                 CancellationToken cancellationToken =
                                                                     default(CancellationToken))
        {
            NamedPipeCommunicationProtocol result;
            if (!actAsClient)
            {
                NamedPipeServerStream writeServer = new NamedPipeServerStream(Path.Combine(address, "server-output"), PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.None);
                NamedPipeServerStream readServer = new NamedPipeServerStream(Path.Combine(address, "server-input"), PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.None);
                await Task.WhenAll(readServer.WaitForConnectionAsync(cancellationToken),
                                   writeServer.WaitForConnectionAsync(cancellationToken))
                          .ConfigureAwait(false);

                result = new NamedPipeCommunicationProtocol(readServer, writeServer, streamFactory, new ServerNameLogDecorator(log,address,false));
            }
            else
            {
                NamedPipeClientStream writeClient = new NamedPipeClientStream(".", Path.Combine(address, "server-input"), PipeDirection.InOut,
                                                                             PipeOptions.None);
                NamedPipeClientStream readClient = new NamedPipeClientStream(".", Path.Combine(address, "server-output"), PipeDirection.InOut,
                                                                              PipeOptions.None);
                await Task.WhenAll(readClient.ConnectAsync(cancellationToken),
                                   writeClient.ConnectAsync(cancellationToken))
                          .ConfigureAwait(false);

                result = new NamedPipeCommunicationProtocol(readClient, writeClient, streamFactory, new ServerNameLogDecorator(log,address,true));
            }
            return result;
        }

        public void SendMessage(Stream message, Action messageCompletedAction = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Stream messageCopy = streamFactory.Create(message.Length);
            message.CopyTo(messageCopy, BufferSize);

            outgoingMessageQueue.Enqueue(messageCopy, messageCompletedAction);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }

        public event EventHandler<EventArgs> CommunicationError;

        protected virtual void OnError()
        {
            CommunicationError?.Invoke(this, EventArgs.Empty);
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
            
            public IncomingMessageQueue(StreamFactory streamFactory, ILog log, PipeStream readStream,
                                        NamedPipeCommunicationProtocol communicationProtocol,
                                        OutgoingMessageQueue outgoingMessageQueue, CancellationToken cancellationToken)
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
                    log.LogVerbose("Entered flush lock.");
                    while (messages.TryDequeue(out IncomingMessage current))
                    {
                        log.LogVerbose($"Flush message {current.Id}.");
                        OnMessageReceived(new MessageReceivedEventArgs(current.Data));
                        current.Dispose();
                    }
                }
                log.LogVerbose("Finished flushing messages. Released lock.");
            }

            private void StartReading()
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        log.LogInformation("Start message reader");
                        byte[] header = new byte[HeaderSize];
                        int headerResult = readStream.Read(header, 0, header.Length);
                        try
                        {
                            if (headerResult == 0)
                            {
                                throw new ClientDisconnectedException();
                            }

                            (int messageLength, bool isSplit, Guid messageGuid, byte confirmation) = ParseHeader(header);

                            if (messageLength > 0)
                            {
                                ReadMessage(messageLength, isSplit, messageGuid, confirmation);
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

                void ReadMessage(int messageLength, bool isSplit, Guid messageGuid, byte confirmation)
                {
                    Stream messageStream = streamFactory.Create(messageLength);
                    int intervals = messageLength / BufferSize;
                    int remaining = messageLength % BufferSize;
                    byte[] buffer = new byte[BufferSize];
                    IncomingMessage message = new IncomingMessage(messageGuid, messageStream, log);

                    try
                    {
                        ReadIntervals();
                        int result = Extensions.ExecutesWithTimeout(() => readStream.Read(buffer, 0, remaining),
                                                                    MaxConfirmationResponseTime);

                        if (result == 0)
                        {
                            throw new ClientDisconnectedException();
                        }

                        if (result != remaining)
                        {
                            throw new PartialMessageException(messageGuid);
                        }

                        AppendBuffer(messageStream, buffer, remaining);
                        messageStream.Seek(0, SeekOrigin.Begin);

                        if (splitMessages.ContainsKey(messageGuid) &&
                            splitMessages.TryGetValue(messageGuid, out IncomingMessage splitMessage))
                        {
                            MergeWithSplitMessage(splitMessage, messageStream);
                            message.Dispose();
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
                        messageStream?.Dispose();
                        message?.Dispose();
                        throw;
                    }
                    catch (TaskCanceledException c)
                    {
                        log.LogError($"Exception during message read.{Environment.NewLine}" +
                                     $"{new MessageTimeoutException(messageGuid.GetHashCode(), MaxConfirmationResponseTime, c)}");
                    }
                    catch (OperationCanceledException oc)
                    {
                        log.LogError($"Exception during message read.{Environment.NewLine}" +
                                     $"{new MessageTimeoutException(messageGuid.GetHashCode(), MaxConfirmationResponseTime, oc)}");
                    }
                    catch (PartialMessageException)
                    {
                        messageStream?.Dispose();
                        message?.Dispose();
                        outgoingMessageQueue.SendMessageConfirmation(messageGuid, false);
                    }
                    catch (Exception e)
                    {
                        log.LogError($"Exception during message read.{Environment.NewLine}" +
                                     $"{e}");
                        messageStream?.Dispose();
                        message?.Dispose();
                        throw new ClientDisconnectedException(e);
                    }
                    
                    log.LogVerbose("Finished reading message.");

                    void ReadIntervals()
                    {
                        while (intervals > 0)
                        {
                            intervals--;
                            ReadInterval();
                        }

                        void ReadInterval()
                        {
                            int result = Extensions.ExecutesWithTimeout(() => readStream.Read(buffer, 0, BufferSize),
                                                                        MaxConfirmationResponseTime);

                            if (result == 0)
                            {
                                throw new ClientDisconnectedException();
                            }

                            if (result != BufferSize)
                            {
                                throw new PartialMessageException(messageGuid);
                            }

                            AppendBuffer(messageStream, buffer, BufferSize);
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
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

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

            protected virtual void MergeWithSplitMessage(IncomingMessage splitMessage, Stream messageStream)
            {
                if (splitMessage == null)
                {
                    throw new ArgumentNullException(nameof(splitMessage));
                }

                if (messageStream == null)
                {
                    throw new ArgumentNullException(nameof(messageStream));
                }

                splitMessage.AppendMessageStream(messageStream, cancellationToken);
                messageStream.Dispose();
            }

            protected virtual void EnqueueSplitMessage(Guid messageGuid, IncomingMessage message)
            {
                splitMessages.TryAdd(messageGuid, message);
            }

            protected virtual void AppendBuffer(Stream messageStream, byte[] buffer, int length)
            {
                if (messageStream == null)
                {
                    throw new ArgumentNullException(nameof(messageStream));
                }

                messageStream.Write(buffer, 0, length);
            }

            protected virtual (int messageLength, bool isSplit, Guid messageGuid, byte confirmation) ParseHeader(byte[] header)
            {
                if (header == null)
                {
                    throw new ArgumentNullException(nameof(header));
                }

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
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    readingThread?.Join(CommunicationConstants.ThreadJoinTimeout);
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
            private readonly NamedPipeCommunicationProtocol communicationProtocol;
            
            private PollingCollectionObserver pollingCollectionObserver;
            private int sending;

            public OutgoingMessageQueue(ILog log, PipeStream writeStream,
                                        NamedPipeCommunicationProtocol communicationProtocol,
                                        CancellationToken cancellationToken)
            {
                this.log = log;
                this.cancellationToken = cancellationToken;
                this.writeStream = writeStream;
                this.communicationProtocol = communicationProtocol;
            }

            public void WaitForFirstMessage()
            {
                pollingCollectionObserver = PollingCollectionObserver.Observe(() =>
                {
                    try
                    {
                        if (Interlocked.Exchange(ref sending, 1) == 1)
                        {
                            return;
                        }
                    
                        while (pendingConfirmations.TryDequeue(out Confirmation confirmation))
                        {
                            confirmation.Send();
                        }

                        if (resendMessages.TryDequeue(out OutgoingMessage resend))
                        {
                            do
                            {
                                SendMessage(resend);
                            } while (resendMessages.TryDequeue(out resend));
                        }
                        else if (unconfirmedMessages.IsEmpty && pendingMessages.TryDequeue(out OutgoingMessage message))
                        {
                            SendMessage(message);
                        }

                        sending = 0;
                        log.LogVerbose("Sending cycle completed. Wait for next message.");
                    }
                    catch (Exception)
                    {
                        //do nothing as this is executed without owning task
                    }
                }, pendingConfirmations, pendingMessages, resendMessages);
                
                void SendMessage(OutgoingMessage message)
                {
                    unconfirmedMessages.TryAdd(message.Id, message);
                    message.SendMessage(cancellationToken);
                }
            }

            public void ConfirmAndContinue(Guid messageId)
            {
                if (unconfirmedMessages.TryRemove(messageId, out OutgoingMessage message))
                {
                    message.ConfirmationTimeElapsed -= OutgoingMessageOnConfirmationTimeElapsed;
                    message.CompleteMessage();
                    message.Dispose();
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
                message.PrepareResend();
                log.LogVerbose($"Resend message {message.Id.ToByteString()} explicitly.");
                resendMessages.Enqueue(message);
            }

            public void Enqueue(Stream message, Action messageCompletedAction)
            {
                Guid messageGuid = Guid.NewGuid();
                OutgoingMessage outgoingMessage = GenerateOutgoingMessage(messageGuid, message, messageCompletedAction);
                outgoingMessage.ConfirmationTimeElapsed += OutgoingMessageOnConfirmationTimeElapsed;
                
                log.LogVerbose($"Enqueue message {outgoingMessage.Id.ToByteString()} with length {outgoingMessage.Data.Length}.");
                pendingMessages.Enqueue(outgoingMessage);
            }

            private void OutgoingMessageOnConfirmationTimeElapsed(object sender, EventArgs e)
            {
                if (sender is OutgoingMessage message && unconfirmedMessages.TryRemove(message.Id, out _))
                {
                    ResendMessage(message);
                }
            }

            protected virtual OutgoingMessage GenerateOutgoingMessage(Guid messageGuid, Stream messageCopy,
                                                                      Action messageCompletedAction)
            {
                return new OutgoingMessage(messageGuid, messageCopy, messageCompletedAction, communicationProtocol, log, writeStream);
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
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    pollingCollectionObserver?.Dispose();

                    foreach (OutgoingMessage message in pendingMessages)
                    {
                        message.ConfirmationTimeElapsed -= OutgoingMessageOnConfirmationTimeElapsed;
                        message.Dispose();
                    }

                    foreach (OutgoingMessage outgoingMessage in unconfirmedMessages.Values.ToArray())
                    {
                        outgoingMessage.ConfirmationTimeElapsed -= OutgoingMessageOnConfirmationTimeElapsed;
                        outgoingMessage.Dispose();
                    }

                    foreach (OutgoingMessage resendMessage in resendMessages)
                    {
                        resendMessage.Dispose();
                    }
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

            public virtual void Send()
            {
                byte[] confirmationHeader = GenerateHeader();
                    
                log.LogInformation($"Send message confirmation for message {messageId.ToByteString()}.");
                writeStream.Write(confirmationHeader, 0, confirmationHeader.Length);
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
            private readonly Timer confirmationTimer = new Timer(MaxConfirmationResponseTime) {AutoReset = false};
            private bool isCompleted;
            private readonly object completedSyncRoot = new object();

            public OutgoingMessage(Guid id, Stream data, Action messageCompletedAction,
                                   NamedPipeCommunicationProtocol communicationProtocol, ILog log,
                                   PipeStream writeStream) : base(id, data, log)
            {
                this.messageCompletedAction = messageCompletedAction;
                this.communicationProtocol = communicationProtocol;
                this.writeStream = writeStream;
                confirmationTimer.Elapsed += ConfirmationTimerOnElapsed;
            }

            private void ConfirmationTimerOnElapsed(object sender, ElapsedEventArgs e)
            {
                OnConfirmationTimeElapsed();
            }

            private bool HasUnsentData => Data.Position < Data.Length;

            protected bool NextMessageIsSplit => Data.Length - Data.Position > MaxMessageLength;

            protected int RemainingLength => (int) (Data.Length - Data.Position);

            public event EventHandler<EventArgs> ConfirmationTimeElapsed; 

            public void SendMessage(CancellationToken cancellationToken)
            {
                sendCounter++;
                if (sendCounter > MaxRetrySendingCount)
                {
                    communicationProtocol.Disconnect();
                    return;
                }

                Log.LogInformation($"Start sending message {Id.ToByteString()}. Attempt nr. {sendCounter}.");

                Data.Seek(0, SeekOrigin.Begin);
                SendMessageInChunks(cancellationToken);
            }

            protected virtual void SendMessageInChunks(CancellationToken cancellationToken)
            {
                while (HasUnsentData)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    byte[] header = GenerateMessageHeader(Id,
                                                          NextMessageIsSplit
                                                              ? SplitMessageIndicator
                                                              : RemainingLength);
                    writeStream.Write(header, 0, header.Length);
                    cancellationToken.ThrowIfCancellationRequested();

                    int sentDataLength = NextMessageIsSplit
                                             ? MaxMessageLength
                                             : RemainingLength;
                    Log.LogVerbose($"Sending data with size {sentDataLength}.");
                    int chunks = sentDataLength / BufferSize;
                    int remaining = sentDataLength % BufferSize;
                    for (int i = 0; i < chunks; i++)
                    {
                        byte[] buffer = new byte[BufferSize];
                        Data.Read(buffer, 0, BufferSize);
                        writeStream.Write(buffer, 0, BufferSize);
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    byte[] remainingBuffer = new byte[remaining];
                    Data.Read(remainingBuffer, 0, remaining);
                    writeStream.Write(remainingBuffer, 0, remaining);
                    Log.LogInformation($"Message {Id.ToByteString()} send.");
                }

                lock (completedSyncRoot)
                {
                    if (!isCompleted)
                    {
                        confirmationTimer.Start();
                    }
                }
            }

            protected virtual byte[] GenerateMessageHeader(Guid messageId, int length)
            {
                Log.LogVerbose($"Sending header with size {length}.");
                return BitConverter.GetBytes(length)
                                   .BigEndian()
                                   .Concat(messageId.ToByteArray())
                                   .Concat(new[] { DefaultConfirmationFlag })
                                   .ToArray();
            }

            public void CompleteMessage()
            {
                lock (completedSyncRoot)
                {
                    if (isCompleted)
                    {
                        return;
                    }

                    isCompleted = true;
                    confirmationTimer.Stop();
                    OnMessageCompleted();
                }
            }

            protected virtual void OnMessageCompleted()
            {
                Log.LogVerbose("Invoke message completed action.");
                messageCompletedAction?.Invoke();
            }

            private void OnConfirmationTimeElapsed()
            {
                ConfirmationTimeElapsed?.Invoke(this, EventArgs.Empty);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    lock (completedSyncRoot)
                    {
                        confirmationTimer.Elapsed -= ConfirmationTimerOnElapsed;
                        confirmationTimer.Dispose();
                    }
                }
                base.Dispose(disposing);
            }

            public void PrepareResend()
            {
                confirmationTimer.Stop();
            }
        }

        protected class IncomingMessage : Message
        {
            public IncomingMessage(Guid id, Stream data, ILog log) : base(id, data, log)
            {
            }

            public void AppendMessageStream(Stream messageStream, CancellationToken token)
            {
                if (messageStream == null)
                {
                    throw new ArgumentNullException(nameof(messageStream));
                }

                int intervals = (int)messageStream.Length / BufferSize;
                int remaining = (int)messageStream.Length % BufferSize;
                byte[] buffer = new byte[BufferSize];

                Data.Seek(0, SeekOrigin.End);
                Data.SetLength(Data.Length + messageStream.Length);
                messageStream.Seek(0, SeekOrigin.Begin);
                token.ThrowIfCancellationRequested();
                for (int i = 0; i < intervals; i++)
                {
                    messageStream.Read(buffer, 0, BufferSize);
                    Data.Write(buffer, 0, BufferSize);
                    token.ThrowIfCancellationRequested();
                }
                messageStream.Read(buffer, 0, remaining);
                Data.Write(buffer, 0, remaining);
                token.ThrowIfCancellationRequested();
            }
        }

        protected abstract class Message : IDisposable
        {
            protected ILog Log { get; }

            protected Message(Guid id, Stream data, ILog log)
            {
                Log = log;
                Id = id;
                Data = data;
            }

            public Stream Data { get; }
            public Guid Id { get; }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Data?.Dispose();
                }
            }
        }
    }
}
