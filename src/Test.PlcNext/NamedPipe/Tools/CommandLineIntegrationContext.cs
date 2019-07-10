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
using Autofac.Core;
using FluentAssertions;
using NSubstitute;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Process;
using PlcNext.NamedPipeServer.CommandLine;
using PlcNext.NamedPipeServer.Tools;

namespace Test.PlcNext.NamedPipe.Tools
{
    public class CommandLineIntegrationContext : SystemTestContext
    {
        private readonly bool withUpdateEnabled;
        private readonly bool withWaitingProcess;
        private readonly string interProcessServerName = Guid.NewGuid().ToByteString();
        private WaitingProcessManager processManager;

        public CommandLineIntegrationContext(PlcNext.SystemTests.Tools.SystemTestContext commandLineContext,
                                             bool withUpdateEnabled = false, bool withWaitingProcess = false)
        {
            this.withUpdateEnabled = withUpdateEnabled;
            this.withWaitingProcess = withWaitingProcess;
            CommandLineContext = commandLineContext;
        }

        public PlcNext.SystemTests.Tools.SystemTestContext CommandLineContext { get; }

        protected override void RegisterCommandLineBuilder(ContainerBuilder builder, Action<string> printMessage)
        {
            if (withUpdateEnabled)
            {
                CommandLineContext.WithUpdateModule(CreateEnvironmentInformation());
            }

            CommandLineContext.Initialize(printMessage,
                                          withWaitingProcess
                                              ? containerBuilder => containerBuilder
                                                                   .RegisterType<WaitingProcessManager>()
                                                                   .As<IProcessManager>()
                                                                   .InstancePerLifetimeScope()
                                                                   .OnActivated(ProcessManagerCreated)
                                              : (Action<ContainerBuilder>) null);
            if (withUpdateEnabled)
            {
                CommandLineContext.WithOtherProgramInstance(13);
                base.RegisterCommandLineBuilder(builder,printMessage);
            }
            else
            {
                builder.RegisterInstance(new CommandLineLifetimeScope(CommandLineContext.Host))
                       .AsSelf()
                       .SingleInstance();
                builder.RegisterType<CommandLineBuilder>().As<ICommandLineBuilder>().InstancePerLifetimeScope();
            }
        }

        private void ProcessManagerCreated(IActivatedEventArgs<WaitingProcessManager> obj)
        {
            processManager = obj.Instance;
        }

        protected override void RegisterEnvironmentInformation(ContainerBuilder builder)
        {
            builder.RegisterInstance(CreateEnvironmentInformation());
        }

        private IEnvironmentInformation CreateEnvironmentInformation()
        {
            IEnvironmentInformation environmentInformationService = Substitute.For<IEnvironmentInformation>();
            if (withUpdateEnabled)
            {
                environmentInformationService.InterProcessServerNameBase.Returns(interProcessServerName);
                environmentInformationService.CurrentProcessId.Returns(13);
            }

            return environmentInformationService;
        }

        public override void Dispose()
        {
            base.Dispose();
            CommandLineContext.Dispose();
        }

        public async Task CheckCanceledInternally()
        {
            await ClientSimulator.WaitForLastMessage();
            processManager.CancellationToken.IsCancellationRequested.Should()
                          .BeTrue("cancellation should have been propagated but was not.");
        }
    }
}