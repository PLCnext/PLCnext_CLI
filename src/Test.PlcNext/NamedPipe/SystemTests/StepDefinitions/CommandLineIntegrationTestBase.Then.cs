#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class CommandLineIntegrationTestBase
    {
        protected void Then_the_command_line_instance_was_canceled_internally()
        {
            ScenarioContext.CheckCanceledInternally();
        }
    }
}