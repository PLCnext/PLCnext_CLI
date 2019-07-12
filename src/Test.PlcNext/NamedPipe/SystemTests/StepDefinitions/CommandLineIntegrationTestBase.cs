#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Test.PlcNext.NamedPipe.Tools;
using Test.PlcNext.Tools.Abstractions.Mocked;
using Xunit.Abstractions;
using SystemTestContext = Test.PlcNext.SystemTests.Tools.SystemTestContext;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class CommandLineIntegrationTestBase : SystemTestBase
    {
        protected CommandLineIntegrationTestBase(ITestOutputHelper helper, bool withUpdates = false, bool withWaitingProcess = false) : base(new CommandLineIntegrationContext(new SystemTestContext(new MockedFileSystemAbstraction(),
                                                                                                                  new MockedDownloadServiceAbstraction(),
                                                                                                                  withWaitingProcess
                                                                                                                      ? null
                                                                                                                      :new MockedProcessManagerAbstraction(),
                                                                                                                  withUpdates
                                                                                                                      ? new MockedUserInterfaceAbstraction()
                                                                                                                      : null,
                                                                                                                  new MockedEnvironmentServiceAbstraction(),
                                                                                                                  new MockedExceptionHandlerAbstraction(),
                                                                                                                  new MockedGuidAbstraction(),
                                                                                                                  new MockedCMakeConversationAbstraction(),
                                                                                                                  false),
                                                                              withUpdates, withWaitingProcess),
            helper)
        {
            
        }

        private new CommandLineIntegrationContext ScenarioContext => (CommandLineIntegrationContext) base.ScenarioContext;
    }
}