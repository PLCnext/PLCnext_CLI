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
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Nito.AsyncEx;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Communication;
using Xunit;

namespace Test.PlcNext.NamedPipe.Tools
{
    public class NamedPipeCommunicationProtocolSimulator : NamedPipeCommunicationProtocol, IClientSimulator
    {
        private readonly PipeStream readStream;
        private readonly PipeStream writeStream;
        private readonly StreamFactory streamFactory;
        private readonly ILog log;
        
        private readonly ConcurrentStack<Stream> messagesReceived = new ConcurrentStack<Stream>();
        private readonly ConcurrentStack<byte> confirmationFlags = new ConcurrentStack<byte>();
        
        private readonly AsyncAutoResetEvent messageResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent messagePreResetEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent aliveEvent = new AsyncAutoResetEvent(false);
        private readonly AsyncAutoResetEvent confirmResetEvent = new AsyncAutoResetEvent(false);
        private readonly ManualResetEvent closedEvent = new ManualResetEvent(false);
        
        public const int DefaultTimeout = 500;

        private int errorConfirmationCount;
        private int skipConfirmationCount;
        private bool wasSplitMessage;

        private NamedPipeCommunicationProtocolSimulator(PipeStream readStream, PipeStream writeStream,
                                                        StreamFactory streamFactory, ILog log)
            : base(readStream, writeStream, streamFactory, log)
        {
            this.readStream = readStream;
            this.writeStream = writeStream;
            this.streamFactory = streamFactory;
            this.log = log;
        }

        public static NamedPipeCommunicationProtocolSimulator Connect(string pipeName, StreamFactory streamFactory, ILog logger)
        {
            NamedPipeClientStream writeClient = new NamedPipeClientStream(".", Path.Combine(pipeName, "server-input"), PipeDirection.InOut,
                                                                          PipeOptions.None);
            NamedPipeClientStream readClient = new NamedPipeClientStream(".", Path.Combine(pipeName, "server-output"), PipeDirection.InOut,
                                                                         PipeOptions.None);
            readClient.Connect();
            writeClient.Connect();

            NamedPipeCommunicationProtocolSimulator result = new NamedPipeCommunicationProtocolSimulator(
                readClient, writeClient, streamFactory, new SimulatorServerNameLogDecorator(logger, pipeName, true));
            result.Start();
            return result;
        }

        public Task WriteMessage(string message, byte[] header = null, int count = 1)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                return WriteMessage(stream, header, count);
            }
        }

        public async Task WriteMessage(Stream data, byte[] header = null, int count = 1)
        {
            AsyncAutoResetEvent sendCompleted = new AsyncAutoResetEvent(false);
            using (FixedHeader(header))
            {
                for (int i = 0; i < count; i++)
                {
                    SendMessage(data, () => sendCompleted.Set());
                }
            }

            Assert.True(await sendCompleted.WaitOne(DefaultTimeout),
                        $"Sending client message did not return after {DefaultTimeout}ms.");
//            sendCompleted.WaitOne(DefaultTimeout)
//                         .Should().BeTrue($"client message should have returned after {DefaultTimeout}ms.");
//            return Task.CompletedTask;
        }

        public async Task WaitForLastMessage(int timeout = DefaultTimeout)
        {
            messageResetEvent.Reset();
            while (await messageResetEvent.WaitOneWithAliveEvent(timeout, aliveEvent))
            {
                //do nothing
            }
        }

        [AssertionMethod]
        public async Task<string> ReadMessage(bool checkTimeout = true,
                                        int timeout = DefaultTimeout)
        {
            messageResetEvent.Reset();
            if (messagesReceived.TryPop(out Stream message))
            {
                return ToString();
            }

            log.LogVerbose($"Client waiting for message with alive event - timeout: {timeout}.");
            bool result = await messageResetEvent.WaitOneWithAliveEvent(timeout, aliveEvent);
            log.LogVerbose($"Waiting for message returned.");
            if (checkTimeout)
            {
                Assert.True(result, "Timeout while reading message.");
            }
            Assert.False(checkTimeout && messagesReceived.IsEmpty, "Read message stack is empty.");
            bool popResult = messagesReceived.TryPop(out message);
            Assert.True(!checkTimeout || popResult, $"Could not pop the message.");
            return ToString();

            string ToString()
            {
                string messageString = message?.ToUtf8String();
                if (string.IsNullOrEmpty(messageString))
                {
                    log.LogWarning("Client read message stream is null. That is weird.");
                }
                else
                {
                    log.LogVerbose($"Client read message returns {messageString}");
                }

                return messageString;
            }
        }

        public void UseErrorConfirmation(int count)
        {
            errorConfirmationCount = count;
        }

        public async Task<bool> WaitForMessages(int messageCount, Func<string, bool> messageFilter = null)
        {
            TimeSpan timeoutTimeSpan = new TimeSpan(0,0,0,0, DefaultTimeout * messageCount);
            DateTime start = DateTime.Now;
            while (DateTime.Now-start < timeoutTimeSpan)
            {
                messageResetEvent.Reset();
                if (MessagesReceived())
                {
                    return true;
                }

                if (!await messageResetEvent.WaitOneWithAliveEvent(DefaultTimeout, aliveEvent))
                {
                    return false;
                }
            }

            return false;

            bool MessagesReceived()
            {
                string[] messages = messagesReceived.ToArray()
                                                    .Select(s => s.ToUtf8String())
                                                    .ToArray();
                return messageFilter != null
                           ? messages.Count(messageFilter) >= messageCount
                           : messages.Length >= messageCount;
            }
        }

        public int CountReceivedMessages(Func<string, bool> messageFilter = null)
        {
            log.LogVerbose($"Start counting messages.");
            string[] messages = messagesReceived.ToArray()
                                                .Select(s => s.ToUtf8String())
                                                .ToArray();
            return messageFilter != null
                       ? messages.Count(messageFilter)
                       : messages.Length;
        }

        public bool WaitForDisconnect()
        {
            return closedEvent.WaitOne(2*DefaultTimeout);
        }

        public void SkipConfirmation(int count)
        {
            skipConfirmationCount = count;
        }

        public new void Disconnect()
        {
            base.Disconnect();
        }

        public async Task<bool> LastMessageWasSplit()
        {
            messagePreResetEvent.Reset();
            if (messagesReceived.Count == 0)
            {
                Assert.True(await messagePreResetEvent.WaitOneWithAliveEvent(DefaultTimeout, aliveEvent),"Timeout while waiting for first message");
            }
            return wasSplitMessage;
        }

        public async Task<byte> GetLastConfirmationFlag()
        {
            confirmResetEvent.Reset();
            while (await confirmResetEvent.WaitOneWithAliveEvent(DefaultTimeout, aliveEvent))
            {
                //do nothing wait for last confirmation flag
            }

            confirmationFlags.Count.Should().BeGreaterThan(0, "a confirmation was expected.");
            return confirmationFlags.TryPop(out byte confirmation) ? confirmation : (byte)0xAB;
        }

        protected override IncomingMessageQueue CreateIncomingMessageQueue(OutgoingMessageQueue outgoingQueue)
        {
            return new SimulatorIncomingMessageQueue(streamFactory, log, readStream, this, CancellationToken,
                                                     outgoingQueue);
        }

        protected override OutgoingMessageQueue CreateOutgoingMessageQueue()
        {
            return new SimulatorOutgoingMessageQueue(log, CancellationToken, writeStream, this);
        }

        protected override void OnError()
        {
            closedEvent.Set();
            base.OnError();
        }

        public override void Dispose()
        {
            foreach (Stream stream in messagesReceived)
            {
                stream.Dispose();
            }
            
            base.Dispose();
        }

        protected override void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            messagePreResetEvent.Set();
            base.OnMessageReceived(sender, e);
            Stream messageCopy = streamFactory.Create(e.Message.Length);
            e.Message.CopyTo(messageCopy);
            messagesReceived.Push(messageCopy);
            messageResetEvent.Set();
        }

        private byte[] fixedHeader;

        private IDisposable FixedHeader(byte[] header)
        {
            fixedHeader = header;
            return new DisposeAction(() => fixedHeader = null);
        }

        private class SimulatorServerNameLogDecorator : ServerNameLogDecorator
        {
            public SimulatorServerNameLogDecorator(ILog logImplementation, string serverName, bool isClient) : base(logImplementation, serverName, isClient)
            {
            }

            protected override string GetPrefix()
            {
                return "simulator-" + base.GetPrefix();
            }
        }

        private class SimulatorOutgoingMessageQueue : OutgoingMessageQueue
        {
            private readonly ILog log;
            private readonly PipeStream writeStream;
            private readonly NamedPipeCommunicationProtocolSimulator simulator;

            public SimulatorOutgoingMessageQueue(ILog log, CancellationToken cancellationToken, PipeStream writeStream,
                                                 NamedPipeCommunicationProtocolSimulator simulator)
                : base(log, cancellationToken, writeStream, simulator)
            {
                this.log = log;
                this.writeStream = writeStream;
                this.simulator = simulator;
            }

            protected override Confirmation CreateConfirmation(Guid messageId, bool success)
            {
                return new SimulatorConfirmation(messageId, success, writeStream, log, simulator);
            }

            protected override OutgoingMessage GenerateOutgoingMessage(Guid messageGuid, Stream messageCopy,
                                                                       Action messageCompletedAction)
            {
                return new SimulatorOutgoingMessage(messageGuid, messageCopy, messageCompletedAction, simulator, log,
                                                    writeStream, simulator.fixedHeader);
            }
        }
        
        private class SimulatorConfirmation : Confirmation
        {
            private readonly Guid messageId;
            private readonly NamedPipeCommunicationProtocolSimulator simulator;

            public SimulatorConfirmation(Guid messageId, bool success, PipeStream writeStream, ILog log, 
                                         NamedPipeCommunicationProtocolSimulator simulator) 
                : base(messageId, success, writeStream, log)
            {
                this.messageId = messageId;
                this.simulator = simulator;
            }

            public override void Send()
            {
                if (simulator.skipConfirmationCount > 0)
                {
                    simulator.skipConfirmationCount--;
                    return;
                }
                base.Send();
            }

            protected override byte[] GenerateHeader()
            {
                if (simulator.errorConfirmationCount == 0)
                {
                    return BitConverter.GetBytes(0)
                                                     .BigEndian()
                                                     .Concat(messageId.ToByteArray())
                                                     .Concat(new[] { SuccessConfirmationFlag })
                                                     .ToArray();
                }

                simulator.errorConfirmationCount--;
                return BitConverter.GetBytes(0)
                                   .BigEndian()
                                   .Concat(messageId.ToByteArray())
                                   .Concat(new[] { ErrorConfirmationFlag })
                                   .ToArray();
            }
        }

        private class SimulatorIncomingMessageQueue : IncomingMessageQueue
        {
            private readonly NamedPipeCommunicationProtocolSimulator communicationProtocol;
            private Guid lastAppendedMessage;

            public SimulatorIncomingMessageQueue(StreamFactory streamFactory, ILog log, PipeStream readStream, NamedPipeCommunicationProtocolSimulator communicationProtocol, CancellationToken cancellationToken, OutgoingMessageQueue outgoingMessageQueue) : base(streamFactory, log, readStream, communicationProtocol, cancellationToken, outgoingMessageQueue)
            {
                this.communicationProtocol = communicationProtocol;
            }

            protected override (int messageLength, bool isSplit, Guid messageGuid, byte confirmation) ParseHeader(byte[] header)
            {
                communicationProtocol.aliveEvent.Set();
                return base.ParseHeader(header);
            }

            protected override void AppendBuffer(Stream messageStream, byte[] buffer, int length)
            {
                communicationProtocol.aliveEvent.Set();
                base.AppendBuffer(messageStream, buffer, length);
            }

            protected override void EnqueueSplitMessage(Guid messageGuid, IncomingMessage message)
            {
                communicationProtocol.aliveEvent.Set();
                base.EnqueueSplitMessage(messageGuid, message);
            }

            protected override void MergeWithSplitMessage(IncomingMessage splitMessage, Stream messageStream)
            {
                lastAppendedMessage = splitMessage.Id;
                base.MergeWithSplitMessage(splitMessage, messageStream);
            }

            protected override void CompleteMessage(IncomingMessage message, Guid messageGuid, byte confirmation)
            {
                if (messageGuid == lastAppendedMessage)
                {
                    communicationProtocol.wasSplitMessage = true;
                }
                base.CompleteMessage(message, messageGuid, confirmation);
            }

            protected override void ProcessConfirmationFlag(byte confirmation, Guid messageGuid)
            {
                communicationProtocol.confirmResetEvent.Set();
                communicationProtocol.confirmationFlags.Push(confirmation);
                base.ProcessConfirmationFlag(confirmation, messageGuid);
            }
        }

        private class SimulatorOutgoingMessage : OutgoingMessage
        {
            private readonly Action messageCompletedAction;
            private readonly PipeStream writeStream;
            private readonly byte[] fixedHeader;

            public SimulatorOutgoingMessage(Guid id, Stream data, Action messageCompletedAction, 
                                            NamedPipeCommunicationProtocol communicationProtocol, 
                                            ILog log, PipeStream writeStream, byte[] fixedHeader) 
                : base(id, data, messageCompletedAction, communicationProtocol, log, writeStream)
            {
                this.messageCompletedAction = messageCompletedAction;
                this.writeStream = writeStream;
                this.fixedHeader = fixedHeader;
            }

            protected override byte[] GenerateMessageHeader(Guid guid, int length)
            {
                return fixedHeader ?? base.GenerateMessageHeader(guid, length);
            }

            protected override void SendMessageInChunks(CancellationToken cancellationToken)
            {
                if (Data.Length == 0)
                {
                    byte[] header = GenerateMessageHeader(Id,
                                                          NextMessageIsSplit
                                                              ? SplitMessageIndicator
                                                              : RemainingLength);
                    Log.LogInformation($"Send header only.{Environment.NewLine}" +
                                       $"{string.Join(", ", header.Select(b => b.ToString("X")))}");
                    writeStream.Write(header, 0, header.Length);
                    Log.LogVerbose("Invoke message completed action.");
                    messageCompletedAction?.Invoke();
                    return;
                }
                
                base.SendMessageInChunks(cancellationToken);
                Log.LogVerbose("Invoke message completed action.");
                messageCompletedAction?.Invoke();
            }

            protected override void OnMessageCompleted()
            {
                //do nothing as it is done prior above
            }
        }
    }
}