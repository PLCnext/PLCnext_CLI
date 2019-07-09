#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using PlcNext.NamedPipeServer.Communication;

namespace PlcNext.NamedPipeServer
{
    internal class ServerConnectionLifetimeScope : IDisposable
    {
        private readonly ICliServer server;
        private readonly ILifetimeScope parentLifetimeScope;
        private ILifetimeScope childLifetimeScope;

        public ServerConnectionLifetimeScope(ICliServer server, ILifetimeScope parentLifetimeScope)
        {
            this.server = server;
            this.parentLifetimeScope = parentLifetimeScope;
            server.Connected += ServerOnConnected;
            server.Disconnected += ServerOnDisconnected;
        }

        private void ServerOnDisconnected(object sender, EventArgs e)
        {
            childLifetimeScope?.Dispose();
        }

        private void ServerOnConnected(object sender, ServerConnectedEventArgs e)
        {
            childLifetimeScope = parentLifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(new CommunicationSettings(e.Heartbeat)).AsSelf();
                builder.RegisterInstance(e.Protocol).As<ICommunicationProtocol>();
                builder.RegisterType<CommandLineMediator>().AsSelf().AutoActivate();
                builder.RegisterType<InterProcessUpdateReceiver>().AsSelf().AutoActivate();
            });
        }

        public void Dispose()
        {
            server.Connected -= ServerOnConnected;
            server.Disconnected -= ServerOnDisconnected;
            childLifetimeScope?.Dispose();
        }
    }
}
