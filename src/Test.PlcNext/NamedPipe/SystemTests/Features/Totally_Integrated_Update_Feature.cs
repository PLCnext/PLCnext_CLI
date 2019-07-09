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
using Xunit;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Checks the feature that other cli instances will trigger an update to the server instance.")]
    public class Totally_Integrated_Update_Feature : CommandLineIntegrationTestBase
    {
        public Totally_Integrated_Update_Feature() : base(true){}
        
        [Scenario]
        public async Task Sending_update_when_the_settings_are_changed()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_change_the_setting_to_the_value("AttributePrefix", "!"),
                _ => Then_the_client_received_a_setting_updated_message()
            ).RunAsync();
        }
        
        [Scenario]
        public async Task Sending_update_when_the_project_targets_are_changed()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152","2.0"),
                _ => Then_the_client_received_a_project_target_updated_message_for_the_project("Standard")
            ).RunAsync();
        }
        
        [Scenario]
        public async Task Sending_update_when_a_sdk_was_explored()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_explore_all_sdks(),
                _ => Then_the_client_received_a_sdks_updated_message()
            ).RunAsync();
        }
        
        [Scenario]
        public async Task Sending_no_update_without_prior_handshake_setting()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client_without_handshake(),
                _ => When_I_change_the_setting_to_the_value("AttributePrefix", "!"),
                _ => Then_the_client_received_no_setting_updated_message()
            ).RunAsync();
        }
        
        [Scenario]
        public async Task Sending_no_update_without_prior_handshake_project_target()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client_without_handshake(),
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152","2.0"),
                _ => Then_the_client_received_no_setting_updated_message()
            ).RunAsync();
        }
        
        [Scenario]
        public async Task Sending_no_update_without_prior_handshake_sdk()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client_without_handshake(),
                _ => When_I_explore_all_sdks(),
                _ => Then_the_client_received_no_setting_updated_message()
            ).RunAsync();
        }
    }
}