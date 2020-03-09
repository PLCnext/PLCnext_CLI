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
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Agents.Net;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.NamedPipeServer.AgentBased.Communication;
using PlcNext.NamedPipeServer.AgentBased.Tools;
using PlcNext.NamedPipeServer.Communication;
using PlcNext.NamedPipeServer.Tools;
using Timer = System.Timers.Timer;
#pragma warning disable 4014

namespace PlcNext.AgentBased.NamedPipeServer.Communication
{
    public class NamedPipeCommunicationProtocolAgent : Agent, IDisposable
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition NamedPipeCommunicationProtocolAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      NamedPipeConnectedMessage.NamedPipeConnectedMessageDefinition,
                                      StreamFactoryCreatedMessage.StreamFactoryCreatedMessageDefinition,
                                      SendMessageRequestedMessage.SendMessageRequestedMessageDefinition
                                  },
                                  new []
                                  {
                                      LogMessage.LogMessageDefinition,
                                      MessageReceivedMessage.MessageReceivedMessageDefinition
                                  });

        #endregion

        public const int HeaderSize = 4;
        public const int BufferSize = 4096;
        public const int MaxPackageSendTime = 10000;
        private bool disconnected;
        private readonly object disconnectedLock = new object();
        private OutgoingMessageQueue outgoingMessageQueue;
        private IncomingMessageQueue incomingMessageQueue;
        private StreamFactory streamFactory;
        private string address;
        private bool isClient;

        private readonly CancellationTokenSource disposedCancellationTokenSource = new CancellationTokenSource();
        protected CancellationToken CancellationToken => disposedCancellationTokenSource.Token;

        public NamedPipeCommunicationProtocolAgent(MessageBoard messageBoard) : base(NamedPipeCommunicationProtocolAgentDefinition, messageBoard)
        {
            connectCollector = new MessageCollector<NamedPipeConnectedMessage, StreamFactoryCreatedMessage>(OnConnectMessagesCollected);
        }

        protected override void ExecuteCore(Agents.Net.Message messageData)
        {
            Contract.Requires(messageData != null, nameof(messageData) + " != null");
            if (messageData.TryGet(out SendMessageRequestedMessage requestedMessage))
            {
                SendMessage(requestedMessage);
                requestedMessage.Consumed();
            }
            else
            {
                connectCollector.Push(messageData);
            }
        }

        #region Start communication
        
        private void OnConnectMessagesCollected(MessageCollection<NamedPipeConnectedMessage, StreamFactoryCreatedMessage> set)
        {
            isClient = set.Message1.IsClient;
            address = set.Message1.Address;
            streamFactory = set.Message2.Factory;
            StartCommunication(set.Message1.ReadStream, set.Message1.WriteStream);
        }

        private readonly MessageCollector<NamedPipeConnectedMessage, StreamFactoryCreatedMessage> connectCollector;

        private void StartCommunication(PipeStream readStream, PipeStream writeStream)
        {
            outgoingMessageQueue = CreateOutgoingMessageQueue(writeStream);
            outgoingMessageQueue.WaitForFirstMessage();
            incomingMessageQueue = CreateIncomingMessageQueue(readStream);
            incomingMessageQueue.MessageReceived += OnMessageReceived;
            incomingMessageQueue.WaitForFirstMessage();
        }

        protected virtual OutgoingMessageQueue CreateOutgoingMessageQueue(PipeStream writeStream)
        {
            return new OutgoingMessageQueue(writeStream, this, CancellationToken);
        }

        protected virtual IncomingMessageQueue CreateIncomingMessageQueue(PipeStream readStream)
        {
            return new IncomingMessageQueue(streamFactory, readStream,
                                            this, 
                                            CancellationToken);
        }

        #endregion

        #region Logging

        private void LogInformation(string message)
        {
            OnMessage(new LogMessage($"{GetPrefix()}: {message}", LogLevel.Information,
                                     Enumerable.Empty<Agents.Net.Message>()));
        }

        private void LogError(string message)
        {
            OnMessage(new LogMessage($"{GetPrefix()}: {message}", LogLevel.Error,
                                     Enumerable.Empty<Agents.Net.Message>()));
        }

        private void LogVerbose(string message)
        {
            OnMessage(new LogMessage($"{GetPrefix()}: {message}", LogLevel.Verbose,
                                     Enumerable.Empty<Agents.Net.Message>()));
        }

        protected virtual string GetPrefix()
        {
            return (isClient ? $"client-{address}" : address);
        }

        #endregion

        #region Send and receive

        private void SendMessage(SendMessageRequestedMessage requestedMessage)
        {
            Stream messageCopy = streamFactory.Create(requestedMessage.Message.Length);
            requestedMessage.Message.CopyTo(messageCopy, BufferSize);

            outgoingMessageQueue.Enqueue(messageCopy, requestedMessage.MessageCompletedAction);
        }

        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if (messageReceivedEventArgs != null)
            {
                OnMessage(new MessageReceivedMessage(messageReceivedEventArgs.Message));
            }
        }

        #endregion

        #region Dispose and Disconnect

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
            
            LogInformation("Disconnecting the pipe stream.");
            disposedCancellationTokenSource.Cancel();
            
            //first disconnect write stream to free other reader and signal shutdown
            outgoingMessageQueue?.ClosePipeStream();
            
            //next dispose reader
            incomingMessageQueue?.ClosePipeStream();
            
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

        private void FlushReceivedMessages()
        {
            LogInformation("Flush all received messages explicitly.");
            incomingMessageQueue.Flush();
        }

        public event EventHandler<EventArgs> CommunicationError;

        protected virtual void OnError()
        {
            CommunicationError?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        protected class IncomingMessageQueue : IDisposable
        {
            private readonly StreamFactory streamFactory;
            private readonly PipeStream readStream;
            private readonly NamedPipeCommunicationProtocolAgent communicationProtocol;
            private readonly ConcurrentQueue<Stream> messages = new ConcurrentQueue<Stream>();
            private readonly CancellationToken cancellationToken;
            private readonly object flushLock = new object();
            
            private Thread readingThread;
            private PollingCollectionObserver pollingCollectionObserver;
            
            public IncomingMessageQueue(StreamFactory streamFactory, PipeStream readStream,
                                        NamedPipeCommunicationProtocolAgent communicationProtocol,
                                        CancellationToken cancellationToken)
            {
                this.streamFactory = streamFactory;
                this.readStream = readStream;
                this.communicationProtocol = communicationProtocol;
                this.cancellationToken = cancellationToken;
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
                    communicationProtocol.LogVerbose("Entered flush lock.");
                    while (messages.TryDequeue(out Stream current))
                    {
                        communicationProtocol.LogVerbose($"Flush message {current.Length}.");
                        OnMessageReceived(new MessageReceivedEventArgs(current));
                        current.Dispose();
                    }
                }
                communicationProtocol.LogVerbose("Finished flushing messages. Released lock.");
            }

            private void StartReading()
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        communicationProtocol.LogInformation("Start message reader");
                        byte[] header = new byte[HeaderSize];
                        int headerResult = readStream.Read(header, 0, header.Length);
                        try
                        {
                            if (headerResult == 0)
                            {
                                throw new ClientDisconnectedException();
                            }

                            int messageLength = ParseHeader(header);

                            if (messageLength > 0)
                            {
                                ReadMessage(messageLength);
                            }
                        }
                        catch (ClientDisconnectedException)
                        {
                            communicationProtocol.Disconnect();
                        }
                    }
                    communicationProtocol.LogInformation("Shutdown reader thread.");
                }
                catch (Exception)
                {
                    //Do not log anything as any log will lead to another exception
                }
            }

            private void ReadMessage(int messageLength)
            {
                Stream messageStream = streamFactory.Create(messageLength);
                int intervals = messageLength / BufferSize;
                int remaining = messageLength % BufferSize;
                byte[] buffer = new byte[BufferSize];

                try
                {
                    ReadIntervals();
                    int result = Extensions.ExecutesWithTimeout(() => readStream.Read(buffer, 0, remaining), MaxPackageSendTime);

                    if (result != remaining)
                    {
                        throw new ClientDisconnectedException();
                    }

                    AppendBuffer(messageStream, buffer, remaining);
                    messageStream.Seek(0, SeekOrigin.Begin);

                    CompleteMessage(messageStream);
                }
                catch (ClientDisconnectedException disconnectedException)
                {
                    communicationProtocol.LogError($"Client disconnected during communication.{Environment.NewLine}" + $"{disconnectedException}");
                    messageStream?.Dispose();
                    throw;
                }
                catch (TaskCanceledException c)
                {
                    communicationProtocol.LogError($"Exception during message read.{Environment.NewLine}" + $"{new MessageTimeoutException(messageLength, MaxPackageSendTime, c)}");
                }
                catch (OperationCanceledException oc)
                {
                    communicationProtocol.LogError($"Exception during message read.{Environment.NewLine}" + $"{new MessageTimeoutException(messageLength, MaxPackageSendTime, oc)}");
                }
                catch (Exception e)
                {
                    communicationProtocol.LogError($"Exception during message read.{Environment.NewLine}" + $"{e}");
                    messageStream?.Dispose();
                    throw new ClientDisconnectedException(e);
                }

                communicationProtocol.LogVerbose("Finished reading message.");

                void ReadIntervals()
                {
                    while (intervals > 0)
                    {
                        intervals--;
                        ReadInterval();
                    }

                    void ReadInterval()
                    {
                        int result = Extensions.ExecutesWithTimeout(() => readStream.Read(buffer, 0, BufferSize), MaxPackageSendTime);

                        if (result != BufferSize)
                        {
                            throw new ClientDisconnectedException();
                        }

                        AppendBuffer(messageStream, buffer, BufferSize);
                    }
                }
            }

            protected virtual void CompleteMessage(Stream message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                LogConcreteMessage();
                message.Seek(0, SeekOrigin.Begin);
                messages.Enqueue(message);
                communicationProtocol.LogInformation($"Received message with {message.Length} bytes.");
                
                void LogConcreteMessage()
                {
                    try
                    {
                        if (message.Length < 1024)
                        {
                            message.Seek(0, SeekOrigin.Begin);
                            //log message text
                            using (StreamReader reader = new StreamReader(message, Encoding.UTF8,
                                                                          true, 1024,
                                                                          true))
                            {
                                communicationProtocol.LogVerbose(
                                    $"Content of received message is{Environment.NewLine}{reader.ReadToEnd()}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        communicationProtocol.LogVerbose($"Error while trying to log received message:{Environment.NewLine}{e}");
                    }
                }
            }

            protected virtual void AppendBuffer(Stream messageStream, byte[] buffer, int length)
            {
                if (messageStream == null)
                {
                    throw new ArgumentNullException(nameof(messageStream));
                }

                messageStream.Write(buffer, 0, length);
            }

            protected virtual int ParseHeader(byte[] header)
            {
                if (header == null)
                {
                    throw new ArgumentNullException(nameof(header));
                }

                return BitConverter.ToInt32(header.BigEndian(), 0);
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

                    foreach (Stream message in messages)
                    {
                        message.Dispose();
                    }
                }
            }

            private void OnMessageReceived(MessageReceivedEventArgs e)
            {
                MessageReceived?.Invoke(this, e);
            }

            public void ClosePipeStream()
            {
                readStream.Close();
                readStream.Dispose();
            }
        }

        protected class OutgoingMessageQueue : IDisposable
        {
            private readonly ConcurrentQueue<OutgoingMessage> pendingMessages = new ConcurrentQueue<OutgoingMessage>();
            private readonly CancellationToken cancellationToken;
            private readonly PipeStream writeStream;
            private readonly NamedPipeCommunicationProtocolAgent communicationProtocol;
            
            private PollingCollectionObserver pollingCollectionObserver;
            private int sending;

            public OutgoingMessageQueue(PipeStream writeStream,
                                        NamedPipeCommunicationProtocolAgent communicationProtocol,
                                        CancellationToken cancellationToken)
            {
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

                        if (pendingMessages.TryDequeue(out OutgoingMessage message))
                        {
                            do
                            {
                                message.SendMessage(cancellationToken);
                            } while (pendingMessages.TryDequeue(out message));
                        }

                        sending = 0;
                        communicationProtocol.LogVerbose("Sending cycle completed. Wait for next message.");
                    }
                    catch (Exception)
                    {
                        //do nothing as this is executed without owning task
                    }
                }, pendingMessages);
            }

            public void Enqueue(Stream message, Action messageCompletedAction)
            {
                Guid messageGuid = Guid.NewGuid();
                OutgoingMessage outgoingMessage = GenerateOutgoingMessage(messageGuid, message, messageCompletedAction);
                
                communicationProtocol.LogVerbose($"Enqueue message {outgoingMessage.Id.ToByteString()} with length {outgoingMessage.Data.Length}.");
                pendingMessages.Enqueue(outgoingMessage);
            }

            protected virtual OutgoingMessage GenerateOutgoingMessage(Guid messageGuid, Stream messageCopy,
                                                                      Action messageCompletedAction)
            {
                return new OutgoingMessage(messageGuid, messageCopy, messageCompletedAction, communicationProtocol, writeStream);
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
                        message.Dispose();
                    }
                }
            }

            public void ClosePipeStream()
            {
                writeStream.Close();
                writeStream.Dispose();
            }
        }

        protected class OutgoingMessage : Message
        {
            private readonly Action messageCompletedAction;
            private readonly NamedPipeCommunicationProtocolAgent communicationProtocol;
            private readonly PipeStream writeStream;

            public OutgoingMessage(Guid id, Stream data, Action messageCompletedAction,
                                   NamedPipeCommunicationProtocolAgent communicationProtocol,
                                   PipeStream writeStream) : base(id, data)
            {
                this.messageCompletedAction = messageCompletedAction;
                this.communicationProtocol = communicationProtocol;
                this.writeStream = writeStream;
            }

            private bool HasUnsentData => Data.Position < Data.Length;

            public void SendMessage(CancellationToken cancellationToken)
            {
                communicationProtocol.LogInformation($"Start sending message {Id.ToByteString()}.");

                Data.Seek(0, SeekOrigin.Begin);
                SendMessageInChunks(cancellationToken);
            }

            protected virtual void SendMessageInChunks(CancellationToken cancellationToken)
            {
                while (HasUnsentData)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    byte[] header = GenerateMessageHeader(Id, (int)Data.Length);
                    writeStream.Write(header, 0, header.Length);
                    cancellationToken.ThrowIfCancellationRequested();

                    int sentDataLength = (int)Data.Length;
                    communicationProtocol.LogVerbose($"Sending data with size {sentDataLength}.");
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
                    communicationProtocol.LogInformation($"Message {Id.ToByteString()} send.");
                }

                communicationProtocol.LogVerbose("Invoke message completed action.");
                messageCompletedAction?.Invoke();
            }

            protected virtual byte[] GenerateMessageHeader(Guid messageId, int length)
            {
                communicationProtocol.LogVerbose($"Sending header with size {length}.");
                return BitConverter.GetBytes(length).BigEndian();
            }
        }

        protected abstract class Message : IDisposable
        {
            protected Message(Guid id, Stream data)
            {
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
