#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.NamedPipe.SystemTests.StepDefinitions;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Checks that the cancel .")]
    [IgnoreScenario("Disabled named pipe communication")]
    public class Totally_Integrated_Cancel_Feature : CommandLineIntegrationTestBase
    {
        public Totally_Integrated_Cancel_Feature(ITestOutputHelper helper) : base(helper, withWaitingProcess:true){}
        
        [Scenario(Timeout = 10000)]
        public async Task Cancel_inside_CLI_on_cancel_command()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => Given_is_the_project_with_set_default_path("Standard"),
                _ => When_I_executed_the_command("build"),
                _ => When_I_cancel_the_command("build"),
                _ => Then_the_command_line_instance_was_canceled_internally()
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Cancel_inside_CLI_on_kill_command()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => Given_is_the_project_with_set_default_path("Standard"),
                _ => When_I_executed_the_command("build"),
                _ => When_I_kill_the_server(),
                _ => Then_the_command_line_instance_was_canceled_internally()
            ).RunAsyncWithTimeout();
        }
    }
}