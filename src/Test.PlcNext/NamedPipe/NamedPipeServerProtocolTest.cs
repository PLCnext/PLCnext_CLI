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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.NamedPipeServer;
using PlcNext.NamedPipeServer.Communication;
using Test.PlcNext.NamedPipe.Tools;
using Xunit;
using Xunit.Abstractions;

namespace Test.PlcNext.NamedPipe
{
    public class NamedPipeServerProtocolTest : IDisposable
    {
        private readonly ICommunicationProtocol protocol;
        private readonly IClientSimulator simulator;
        private const string DefaultMessage = "Lorem Lipsum";
        private readonly ConcurrentStack<Stream> serverMessages = new ConcurrentStack<Stream>();
        private readonly ManualResetEvent serverMessageReceived = new ManualResetEvent(false);
        private readonly ManualResetEvent serverError = new ManualResetEvent(false);
        private readonly StreamFactory streamFactory;

        public NamedPipeServerProtocolTest(ITestOutputHelper output)
        {
            streamFactory = PageStreamFactory.CreateDefault();
            string serverName = Guid.NewGuid().ToByteString();
            Task<ICommunicationProtocol> creationTask = NamedPipeCommunicationProtocol.Connect(serverName, streamFactory, new LogTracer(output));
            simulator = NamedPipeCommunicationProtocolSimulator.Connect(serverName, streamFactory, new LogTracer(output));
            creationTask.Wait();
            protocol = creationTask.Result;
            protocol.MessageReceived += OnMessageReceived;
            protocol.Error += OnError;
            protocol.Start();
        }

        [Fact(Timeout = 10000)]
        public async Task SendMessageIsReceivedByTheClient()
        {
            protocol.SendMessage(DefaultMessage);
            string receivedMessage = await simulator.ReadMessage();

            Assert.Equal(DefaultMessage, receivedMessage);
        }

        [Fact(Timeout = 10000)]
        public async Task ReceiveMessagesFromClient()
        {
            await simulator.WriteMessage(DefaultMessage);

            Assert.Equal(DefaultMessage, GetLatestServerMessage());
        }

        [Fact(Timeout = 10000)]
        public async Task ConfirmMessageFromClient()
        {
            await simulator.WriteMessage(DefaultMessage);

            Assert.Equal(NamedPipeCommunicationProtocol.SuccessConfirmationFlag, await simulator.GetLastConfirmationFlag());
        }

        [Fact(Timeout = 10000)]
        public async Task ErrorConfirmationOnCorruptedMessage()
        {
            byte[] header = GenerateCorruptedHeader(DefaultMessage);
            await simulator.WriteMessage(DefaultMessage, header);

            Assert.Equal(NamedPipeCommunicationProtocol.ErrorConfirmationFlag, await simulator.GetLastConfirmationFlag());
        }

        [Fact(Timeout = 10000)]
        public async Task ErrorConfirmationOnMissingMessageAfterHeader()
        {
            byte[] header = GenerateCorruptedHeader(DefaultMessage);
            await simulator.WriteMessage(string.Empty, header);

            Assert.Equal(NamedPipeCommunicationProtocol.ErrorConfirmationFlag, await simulator.GetLastConfirmationFlag());
        }

        [Fact(Timeout = 10000)]
        public async Task ErrorConfirmationOnMissingMessageAfterBufferSize()
        {
            byte[] header = GenerateCorruptedHeader(2*NamedPipeCommunicationProtocol.BufferSize);
            await simulator.WriteMessage(streamFactory.GenerateRandomStream(NamedPipeCommunicationProtocol.BufferSize), header);

            Assert.Equal(NamedPipeCommunicationProtocol.ErrorConfirmationFlag, await simulator.GetLastConfirmationFlag());
        }

        [Fact(Timeout = 10000)]
        public async Task ErrorConfirmationOnPartialMessageChunk()
        {
            byte[] header = GenerateCorruptedHeader(2 * NamedPipeCommunicationProtocol.BufferSize);
            Stream generatedStream = streamFactory.GenerateRandomStream(NamedPipeCommunicationProtocol.BufferSize +
                                                                        NamedPipeCommunicationProtocol.BufferSize / 2);
            await simulator.WriteMessage(generatedStream, header);

            Assert.Equal(NamedPipeCommunicationProtocol.ErrorConfirmationFlag, await simulator.GetLastConfirmationFlag());
        }

        [Fact(Timeout = 10000)]
        public async Task ServerResendsMessagesWithErrorConfirmationThreeTimes()
        {
            simulator.UseErrorConfirmation(NamedPipeCommunicationProtocol.MaxRetrySendingCount-1);
            protocol.SendMessage(DefaultMessage);

            Assert.True(await simulator.WaitForMessages(NamedPipeCommunicationProtocol.MaxRetrySendingCount, m => m == DefaultMessage), "Did not receive message three times.");
        }

        [Fact(Timeout = 10000)]
        public async Task ServerResendsMessagesWithTimeoutThreeTimes()
        {
            simulator.SkipConfirmation(NamedPipeCommunicationProtocol.MaxRetrySendingCount-1);
            protocol.SendMessage(DefaultMessage);

            Assert.True(await simulator.WaitForMessages(NamedPipeCommunicationProtocol.MaxRetrySendingCount, m => m == DefaultMessage), "Did not receive message three times.");
        }

        [Fact(Timeout = 10000)]
        public void ServerDisconnectsAfterThreeFailedAttempts()
        {
            simulator.UseErrorConfirmation(NamedPipeCommunicationProtocol.MaxRetrySendingCount);
            protocol.SendMessage(DefaultMessage);

            Assert.True(simulator.WaitForDisconnect(), "Server did not disconnect.");
            Assert.True(serverError.WaitOne(100), "Server did not raise error event.");
        }

        [Fact(Timeout = 10000)]
        public void ServerClosesAfterClientDisconnect()
        {
            simulator.Disconnect();

            Assert.True(serverError.WaitOne(200), "Server did not register disconnect.");
        }

        #region Helpers

        private byte[] GenerateCorruptedHeader(string message)
        {
            return GenerateCorruptedHeader(Encoding.UTF8.GetBytes(message).Length);
        }

        private byte[] GenerateCorruptedHeader(int length)
        {
            return BitConverter.GetBytes(length + 1)
                               .BigEndian()
                               .Concat(Guid.NewGuid().ToByteArray())
                               .Concat(new byte[] { NamedPipeCommunicationProtocol.DefaultConfirmationFlag })
                               .ToArray();
        }

        private string GetLatestServerMessage()
        {
            serverMessageReceived.Reset();
            if (serverMessages.TryPop(out Stream message))
            {
                string result = Encoding.UTF8.GetString(message.ReadToEnd());
                message.Dispose();
                return result;
            }

            Assert.True(serverMessageReceived.WaitOne(500), "Server did not receive messages during timeout");
            serverMessages.TryPop(out message);
            string stringResult = Encoding.UTF8.GetString(message.ReadToEnd());
            message.Dispose();
            return stringResult;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Stream message = streamFactory.Create(e.Message.Length);
            e.Message.CopyTo(message);
            serverMessages.Push(message);
            serverMessageReceived.Set();
        }

        private void OnError(object sender, EventArgs e)
        {
            serverError.Set();
        }

        public void Dispose()
        {
            while (serverMessages.TryPop(out Stream message))
            {
                message.Dispose();
            }
            protocol.MessageReceived -= OnMessageReceived;
            protocol.Dispose();
            simulator.Dispose();
        }

        #endregion
    }
}
