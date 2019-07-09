#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Tools;
#pragma warning disable 4014

namespace PlcNext.NamedPipeServer.Communication
{
    internal class InterProcessUpdateReceiver : IDisposable
    {
        private readonly string serverName;
        private readonly StreamFactory streamFactory;
        private readonly ILog log;
        private readonly ICommunicationProtocol externalProtocol;
        private readonly IMessageParser messageParser;

        private readonly CancellationTokenSource disposeSource = new CancellationTokenSource();
        private CancellationToken CancellationToken => disposeSource.Token;
        
        private ICommunicationProtocol protocol;
        private bool handshakeCompleted;
        private Thread updateThread;
        
        public InterProcessUpdateReceiver(IEnvironmentInformation environmentInformation, StreamFactory streamFactory, ILog log, ICommunicationProtocol externalProtocol, IMessageParser messageParser)
        {
            serverName = environmentInformation.InterProcessServerNameBase +
                         environmentInformation.CurrentProcessId.ToString("D");
            this.streamFactory = streamFactory;
            this.log = log;
            this.externalProtocol = externalProtocol;
            this.messageParser = messageParser;
            messageParser.HandshakeCompleted += MessageParserOnHandshakeCompleted;
            StartAndWaitForInterProcessUpdate();
        }

        private void MessageParserOnHandshakeCompleted(object sender, HandshakeEventArgs e)
        {
            handshakeCompleted = true;
        }

        private void StartAndWaitForInterProcessUpdate()
        {
            updateThread = new Thread(StartAndWaitForInterProcessUpdateInternal)
            {
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };
            updateThread.Start();
        }

        private async void StartAndWaitForInterProcessUpdateInternal()
        {
            try
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    protocol?.Dispose();
                    log.LogVerbose($"Start other instance update receiver {serverName}.");

                    protocol = await NamedPipeCommunicationProtocol.Connect(serverName, streamFactory, log,
                                                                            cancellationToken: CancellationToken);
                    log.LogVerbose($"Other instance connected to the update receiver {serverName}.");
                    AutoResetEventAsync waitEvent = new AutoResetEventAsync(false);
                    protocol.Error += ProtocolOnError;
                    protocol.MessageReceived += ProtocolOnMessageReceived;
                    protocol.Start();
                    await waitEvent.WaitAsync(CancellationToken);

                    void ProtocolOnError(object sender, EventArgs e)
                    {
                        protocol.FlushReceivedMessages();
                        protocol.Error -= ProtocolOnError;
                        protocol.MessageReceived -= ProtocolOnMessageReceived;
                        waitEvent.Set();
                    }
                }
            }
            catch (Exception)
            {
                //Do not log anything as any log will lead to another exception
            }
        }

        private void ProtocolOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!handshakeCompleted)
            {
                log.LogInformation("Received update message prior to handshake. The message will not be passed to the client.");
                return;
            }

            log.LogVerbose($"Update receiver {serverName} forwarding message from other instance.");
            externalProtocol.SendMessage(e.Message);
        }

        public void Dispose()
        {
            disposeSource.Cancel();
            updateThread.Join();
            
            messageParser.HandshakeCompleted -= MessageParserOnHandshakeCompleted;
            protocol?.Dispose();
        }
    }
}