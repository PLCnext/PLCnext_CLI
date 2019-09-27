#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Communication;
using ExecutionContext = PlcNext.Common.Tools.ExecutionContext;
#pragma warning disable 4014

namespace PlcNext.CommandLine
{
    [Verb("start-client", HelpText = "Starts the CLI as a named pipe client.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class StartClientVerb : VerbBase
    {
        [Option(CommandLineConstants.ServerNameChar, CommandLineConstants.ServerNameOption,
            HelpText = "The name/address of the server.",
            Required = true)]
        public string ServerName { get; set; }
        
        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            ExecutionContext context = LifetimeScope.Resolve<ExecutionContext>();
            context.WriteInformation(string.Format(CultureInfo.InvariantCulture, MessageResources.ClientStarting, ServerName));
            using (ICommunicationProtocol protocol = await NamedPipeCommunicationProtocol.Connect(ServerName,
                                                                                        LifetimeScope.Resolve<StreamFactory>(),
                                                                                        LifetimeScope.Resolve<ILog>(),
                                                                                        actAsClient: true)
                                                                                         .ConfigureAwait(false))
            using (ManualResetEvent serverStoppedEvent = new ManualResetEvent(false))
            {
                context.WriteInformation(MessageResources.ClientStarted);
                protocol.CommunicationError += OnError;
                protocol.MessageReceived += OnMessageReceived;
                protocol.Start();

                Task.Run(ReadConsoleAsync);
                serverStoppedEvent.WaitOne(-1, true);

                return 0;
                
                void OnError(object sender, EventArgs e)
                {
                    protocol.CommunicationError -= OnError;
                    context.WriteInformation(MessageResources.ClientServerDisconnectedMessage);
                    serverStoppedEvent.Set();
                }
                
                void OnMessageReceived(object sender, MessageReceivedEventArgs e)
                {
                    context.WriteInformation(MessageResources.ClientMessageReceived);
                    context.WriteInformation(Encoding.UTF8.GetString(e.Message.ReadToEnd()));
                }

                void ReadConsoleAsync()
                {
                    string serverMessage = Console.ReadLine();
                    if (serverMessage?.Equals("kill",StringComparison.OrdinalIgnoreCase) == true)
                    {
                        serverStoppedEvent.Set();
                    }
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serverMessage)))
                    {
                        context.WriteInformation(MessageResources.ClientSendingMessage);
                        protocol.SendMessage(stream);
                    }

                    Task.Run(ReadConsoleAsync);
                }
            }
        }
    }
}