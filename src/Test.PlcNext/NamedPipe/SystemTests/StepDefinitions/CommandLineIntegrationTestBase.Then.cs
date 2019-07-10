#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class CommandLineIntegrationTestBase
    {
        protected async Task Then_the_command_line_instance_was_canceled_internally()
        {
            await ScenarioContext.CheckCanceledInternally();
        }
    }
}