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
using Autofac;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;

namespace PlcNext.NamedPipeServer.Communication
{
    public class NamedPipeInstanceCommunicationChannel : ITemporaryCommunicationChannel
    {
        private readonly ILifetimeScope lifetimeScope;

        private NamedPipeInstanceCommunicationChannel(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public static async Task<NamedPipeInstanceCommunicationChannel> OpenChannel(
            string serverAddress, StreamFactory streamFactory, IContainer parentContainer, ILog channelLog, CancellationToken cancellationToken)
        {
            ICommunicationProtocol protocol;
            using (CancellationTokenSource timeout = new CancellationTokenSource(NamedPipeCommunicationProtocol.MaxConfirmationResponseTime))
            using (CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,timeout.Token))
            {
                protocol = await NamedPipeCommunicationProtocol.Connect(serverAddress, streamFactory,
                                                               channelLog,
                                                               cancellationToken: linkedSource.Token,
                                                               actAsClient: true);
                protocol.Start();
            }
            channelLog.LogVerbose($"Successfully connected to other instance server {serverAddress}.");
            ILifetimeScope lifetimeScope = parentContainer.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(channelLog).As<ILog>().SingleInstance();
                builder.RegisterInstance(protocol).As<ICommunicationProtocol>().SingleInstance();
                builder.RegisterInstance(new CommunicationSettings(false)).AsSelf();
            });
            return new NamedPipeInstanceCommunicationChannel(lifetimeScope);
        }

        public IInstanceMessageSender MessageSender => lifetimeScope.Resolve<IMessageSender>();

        public void Dispose()
        {
            lifetimeScope?.Dispose();
        }
    }
}