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
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using CommandLine.Text;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.CommandLine;
using PlcNext.NamedPipeServer.Communication;
using ExecutionContext = PlcNext.Common.Tools.ExecutionContext;

namespace PlcNext.CommandLine
{

    [Verb(CommandLineConstants.ServerVerb, HelpText = "Starts the CLI as a named pipe server.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class StartServerVerb : VerbBase
    {
        [Option(CommandLineConstants.ServerNameChar, CommandLineConstants.ServerNameOption,
            HelpText = "The name/address the server should use. It is best to use a unique value like a GUID.",
            Required = true)]
        public string ServerName { get; set; }

        [Option(CommandLineConstants.HeartbeatChar, CommandLineConstants.HeartbeatOption, HelpText =
                "With this option the server will send a heartbeat at least every 100 ms. " +
                "Without the heartbeat it will be more difficult to check whether the server is not stuck. " +
                "Therefore it is best to use this option.",
            Required = false)]
        public bool HeartbeatEnabled { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> StartServerExample =>
            new[]
            {
                new UsageExample("Start the server with the address 'my_guid' and heartbeat enabled:",
                                 $"{CommandLineConstants.ServerVerb} --{CommandLineConstants.ServerNameOption}  my_guid --{CommandLineConstants.HeartbeatOption}"),
            };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            ExecutionContext context = LifetimeScope.Resolve<ExecutionContext>();
            ContainerBuilder serverBuilder = new ContainerBuilder();
            serverBuilder.RegisterModule<NamedPipeServer.DiModule>();
            serverBuilder.RegisterInstance(new CommandLineLifetimeScope(LifetimeScope))
                         .AsSelf();
            serverBuilder.RegisterInstance(LifetimeScope.Resolve<ILog>()).As<ILog>();
            using (IContainer serverHost = serverBuilder.Build())
            using (ManualResetEvent serverStoppedEvent = new ManualResetEvent(false))
            {
                ICliServer server = serverHost.Resolve<ICliServer>();
                server.Disconnected += ServerOnDisconnected;
                context.WriteInformation(string.Format(CultureInfo.InvariantCulture, MessageResources.StartingServerMessage, ServerName));
                bool result = await server.Start(ServerName, HeartbeatEnabled).ConfigureAwait(false);
                if (!result)
                {
                    return -1;
                }
                context.WriteInformation(MessageResources.ServerStartedMessage);

                serverStoppedEvent.WaitOne(-1, true);
                
                void ServerOnDisconnected(object sender, EventArgs e)
                {
                    server.Disconnected -= ServerOnDisconnected;
                    serverStoppedEvent.Set();
                }
            }
            
            context.WriteInformation(MessageResources.ServerStopped);
            return 0;
        }
    }
}