#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Agents.Net;
using PlcNext.NamedPipeServer.Communication;

namespace PlcNext.NamedPipeServer.AgentBased.Communication
{
    public sealed class NamedPipeConnectorAgent : Agent, IDisposable
    {
        #region Definition

        [AgentDefinition]
        public static AgentDefinition NamedPipeConnectorAgentDefinition { get; }
            = new AgentDefinition(new []
                                  {
                                      NamedPipeClientStartRequestedMessage.NamedPipeClientStartRequestedMessageDefinition,
                                      NamedPipeServerStartRequestedMessage.NamedPipeServerStartRequestedMessageDefinition
                                  },
                                new []
                                {
                                    NamedPipeConnectedMessage.NamedPipeConnectedMessageDefinition
                                });

        #endregion

        private readonly CancellationTokenSource disposeSource = new CancellationTokenSource();

        public NamedPipeConnectorAgent(MessageBoard messageBoard) : base(NamedPipeConnectorAgentDefinition, messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            Contract.Requires(messageData != null, nameof(messageData) + " != null");

            if (messageData.TryGet(out NamedPipeClientStartRequestedMessage clientStart))
            {
                StartClient(clientStart);
            }
            else
            {
                StartServer(messageData.Get<NamedPipeServerStartRequestedMessage>());
            }
        }

        private void StartServer(NamedPipeServerStartRequestedMessage serverStart)
        {
            NamedPipeServerStream writeServer = new NamedPipeServerStream(Path.Combine(serverStart.Address, "server-output"), PipeDirection.InOut, 1,
                                                                          PipeTransmissionMode.Byte,
                                                                          PipeOptions.None);
            NamedPipeServerStream readServer = new NamedPipeServerStream(Path.Combine(serverStart.Address, "server-input"), PipeDirection.InOut, 1,
                                                                         PipeTransmissionMode.Byte,
                                                                         PipeOptions.None);
            Task.WhenAll(readServer.WaitForConnectionAsync(disposeSource.Token),
                         writeServer.WaitForConnectionAsync(disposeSource.Token))
                .ConfigureAwait(false).GetAwaiter().GetResult();

            OnMessage(new NamedPipeConnectedMessage(readServer, writeServer, serverStart.Address, false, serverStart));
        }

        private void StartClient(NamedPipeClientStartRequestedMessage clientStart)
        {
            NamedPipeClientStream writeClient = new NamedPipeClientStream(".", Path.Combine(clientStart.Address, "server-input"), PipeDirection.InOut,
                                                                          PipeOptions.None);
            NamedPipeClientStream readClient = new NamedPipeClientStream(".", Path.Combine(clientStart.Address, "server-output"), PipeDirection.InOut,
                                                                         PipeOptions.None);
            Task.WhenAll(readClient.ConnectAsync(disposeSource.Token),
                         writeClient.ConnectAsync(disposeSource.Token))
                .ConfigureAwait(false).GetAwaiter().GetResult();

            OnMessage(new NamedPipeConnectedMessage(readClient, writeClient, clientStart.Address, true, clientStart));
        }

        public void Dispose()
        {
            disposeSource?.Cancel();
            disposeSource?.Dispose();
        }
    }
}
