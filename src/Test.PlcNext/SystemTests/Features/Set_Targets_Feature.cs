#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.StepDefinitions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Sets targets.")]
    public class Set_Targets_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Add_single_target_to_project_with_targets_adds_target()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152","2.0"),
                _ => Then_the_project_supports_the_targets("AXCF2152,1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)"
                                                         , "AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario]
        public async Task Add_two_targets_to_project_with_targets_adds_target_check_sorted()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152","20.6"),
                _ => When_I_add_the_target("AXCF2152","2.0"),
                _ => Then_the_project_supports_the_targets_sorted("AXCF2152,1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)"
                                                                , "AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)"
                                                                , "AXCF2152,20.6 LTS (20.6.0.12345 branches/release/20.6.0/ beta)")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_single_target_to_project_with_unsorted_targets_adds_target_check_sorted()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithUnsortedTargets"),
                _ => When_I_add_the_target("AXCF2152", "20.6"),
                _ => Then_the_project_supports_the_targets_sorted("AXCF2152,1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)"
                                                                , "AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)"
                                                                , "AXCF2152,20.6 LTS (20.6.0.12345 branches/release/20.6.0/ beta)")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_single_target_to_project_without_targets_adds_target()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_add_the_target("AXCF2152,2.0", null),
                _ => Then_the_project_supports_the_targets("AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario]
        public async Task Remove_single_target_from_project_with_multiple_targets_removes_target()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithMetadata"),
                _ => When_I_remove_the_target("AXCF2152","1.0"),
                _ => Then_the_project_supports_the_targets("AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_single_target_from_project_with_single_target_removes_target()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_remove_the_target("AXCF2152","1.0"),
                _ => Then_the_project_supports_the_targets()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_single_target_from_project_without_targets_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_remove_the_target("AXCF2152", null),
                _ => Then_the_user_was_informed_that_the_target_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_non_existing_target_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_remove_the_target("AXC", null),
                _ => Then_the_user_was_informed_that_the_target_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_target_on_project_without_proj_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentInSourceFolder"),
                _ => When_I_add_the_target("AXCF2152", null),
                _ => Then_the_user_was_informed_that_a_target_can_not_be_set_for_this_project()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_target_without_add_remove_change_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_set_the_target("AXCF2152"),
                _ => Then_the_user_was_informed_that_an_option_must_be_provided()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_already_supported_target_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152", "1.0"),
                _ => Then_the_user_was_informed_that_the_target_is_already_supported()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Specify_only_prefix_of_version_adds_target()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_add_the_target("AXCF2152", "2.0"),
                _ => Then_the_project_supports_the_targets("AXCF2152,1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)"
                                                         , "AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)")
                ).RunAsyncWithTimeout();
        }
    }
}
