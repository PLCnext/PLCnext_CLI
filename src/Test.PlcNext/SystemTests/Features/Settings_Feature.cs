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
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.StepDefinitions;
#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Sets settings.")]
    public class Settings_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Set_single_setting_changes_it()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("AttributePrefix", "!"),
                _ => Then_the_setting_has_the_value("AttributePrefix", "!")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_bool_setting_changes_it()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("UseSystemCommands", "true"),
                _ => Then_the_setting_has_the_value("UseSystemCommands", "True")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_collection_setting_changes_it()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => Then_the_setting_has_the_values("TemplateLocations",
                                                     IsWindowsSystem ? "C:\\foo\\ba" : "C:/foo/ba",
                                                     IsWindowsSystem ? "C:\\other\\foo" : "C:/other/foo")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_to_collection_setting_adds_value()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => When_I_add_the_value_to_the_setting_collection("TemplateLocations", "C:/new/path"),
                _ => Then_the_setting_has_the_values("TemplateLocations",
                                                     IsWindowsSystem ? "C:\\foo\\ba" : "C:/foo/ba",
                                                     IsWindowsSystem ? "C:\\other\\foo" : "C:/other/foo",
                                                     IsWindowsSystem ? "C:\\new\\path" : "C:/new/path")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_from_collection_setting_removes_value()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => When_I_remove_the_value_from_the_setting_collection("TemplateLocations", "C:/foo/ba"),
                _ => Then_the_setting_has_the_values("TemplateLocations", IsWindowsSystem ? "C:\\other\\foo" : "C:/other/foo")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Clear_collection_setting_removes_all_values()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => When_I_clear_the_setting("TemplateLocations"),
                _ => Then_the_setting_has_the_values("TemplateLocations")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Clear_collection_setting_with_value_removes_all_values()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => When_I_clear_the_setting_with_the_value("TemplateLocations", "C:/foo/ba"),
                _ => Then_the_setting_has_the_values("TemplateLocations")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_non_existing_single_setting_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("FooBa", "Value"),
                _ => Then_the_user_was_informed_that_the_setting_does_not_exist()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_value_to_single_setting_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_add_the_value_to_the_setting_collection("AttributePrefix", "!"),
                _ => Then_the_user_was_informed_that_the_setting_is_no_collection()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_value_to_single_setting_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_remove_the_value_from_the_setting_collection("AttributePrefix", "!"),
                _ => Then_the_user_was_informed_that_the_setting_is_no_collection()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Clear_value_of_single_setting_deletes_value()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_clear_the_setting("AttributePrefix"),
                _ => Then_the_setting_has_the_value("AttributePrefix", "")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_single_setting_without_value_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_without_any_value("AttributePrefix"),
                _ => Then_the_user_was_informed_that_a_value_is_necessary()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Add_to_collection_setting_without_value_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_add_without_any_value_to_the_setting_collection("TemplateLocations"),
                _ => Then_the_user_was_informed_that_a_value_is_necessary()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_from_collection_setting_without_value_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_remove_without_any_value_from_the_setting_collection("TemplateLocations"),
                _ => Then_the_user_was_informed_that_a_value_is_necessary()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Remove_from_collection_setting_non_existing_value_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("TemplateLocations", "C:/foo/ba;C:/other/foo"),
                _ => When_I_remove_the_value_from_the_setting_collection("TemplateLocations", "C:/foo/bar"),
                _ => Then_the_user_was_informed_that_the_value_does_not_exist_and_asked_if_the_value_SUGGESTION_was_suggested(IsWindowsSystem ? "C:\\foo\\ba" : "C:/foo/ba")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Set_bool_setting_with_wrong_value_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_are_the_standard_settings(),
                _ => When_I_change_the_setting_to_the_value("UseSystemCommands", "Schnuden"),
                _ => Then_the_user_was_informed_that_only_bool_values_are_allowed()
            ).RunAsyncWithTimeout();
        }
    }
}
