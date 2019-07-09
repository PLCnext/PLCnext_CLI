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
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;

namespace PlcNext.NamedPipeServer.Communication
{
    internal class CliServer : ICliServer, IDisposable
    {
        private readonly StreamFactory streamFactory;
        private readonly ILog log;
        private readonly object syncRoot = new object();

        private ICommunicationProtocol communicationProtocol;
        private CancellationTokenSource startCancellationTokenSource;

        public CliServer(StreamFactory streamFactory, ILog log)
        {
            this.streamFactory = streamFactory;
            this.log = log;
        }

        public async Task<bool> Start(string serverName, bool twoChannelCommunication, bool heartbeat)
        {
            if (communicationProtocol != null)
            {
                Stop();
            }

            try
            {
                startCancellationTokenSource = new CancellationTokenSource();
                communicationProtocol =
                    await NamedPipeCommunicationProtocol.Connect(serverName, streamFactory, log, twoChannelCommunication,
                                                        startCancellationTokenSource.Token);
            }
            catch (TaskCanceledException e)
            {
                log.LogError($"Connection to server was canceled.{Environment.NewLine}{e}");
                communicationProtocol = null;
                return false;
            }
            finally
            {
                startCancellationTokenSource.Dispose();
                startCancellationTokenSource = null;
            }

            if (communicationProtocol != null)
            {
                communicationProtocol.Error += CommunicationProtocolOnError;
            }
            OnConnected(new ServerConnectedEventArgs(communicationProtocol, heartbeat));
            communicationProtocol.Start();
            return true;
        }

        private void CommunicationProtocolOnError(object sender, EventArgs e)
        {
            Stop();
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                if (communicationProtocol != null)
                {
                    communicationProtocol.Error -= CommunicationProtocolOnError;
                }
                startCancellationTokenSource?.Cancel();
                communicationProtocol?.Dispose();
                communicationProtocol = null;
                OnDisconnected();
            }
        }

        public event EventHandler<ServerConnectedEventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;

        public void Dispose()
        {
            Stop();
        }

        protected virtual void OnConnected(ServerConnectedEventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
