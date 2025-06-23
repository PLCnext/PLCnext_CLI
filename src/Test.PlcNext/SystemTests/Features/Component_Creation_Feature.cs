﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.StepDefinitions;
using Test.PlcNext.Tools;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"User creates new component via CLI with different parameters.")]
    public class Component_Creation_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Create_new_component_with_specific_name()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_component_with_name("NewComponent", false),
                _ => Then_the_user_was_informed_that_the_component_was_created_successfully(),
                _ => Then_the_project_contains_a_component_with_name("NewComponent")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_for_project_in_specific_folder()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_create_a_new_component_with_name_for_project_in_specific_folder("NewComponent", false),
                _ => Then_the_user_was_informed_that_the_component_was_created_successfully(),
                _ => Then_the_project_contains_a_component_with_name("NewComponent")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_with_existing_name_forced()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_component(true, false),
                _ => Then_the_project_contains_a_component_with_name("StandardComponent"),
                _ => Then_the_user_was_informed_that_the_component_was_created_successfully(),
                _ => Then_the_code_entity_was_created()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_creates_component_with_correct_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_component(true, true),
                _ => Then_the_user_was_informed_that_the_component_was_created_successfully(),
                _ => Then_the_components_namespace_starts_with_namespace("DComponent", "A.B.C.D")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_with_custom_relative_path()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_component_with_path("src/customfolder", true, false),
                _ => Then_the_component_exists_in_path("DComponent", "A.B.C.D/src/customfolder")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_with_custom_absolute_path()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_component_with_path(Path.GetFullPath("A.B.C.D"), true, false),
                _ => Then_the_component_exists_in_path("DComponent", "A.B.C.D")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm"),
                _ => Given_is_the_working_directory_PATH("Arp.Plc.Esm"),
                _ => When_I_create_a_new_component(false, false),
                _ => Then_the_components_namespace_starts_with_namespace("EsmComponent", "Arp.Plc.Esm")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Inform_user_of_incompatible_template_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_create_a_new_component_with_name("PlmComponent", false),
                _ => Then_the_user_was_informed_that_the_template_is_incompatible())
                        .RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Inform_user_of_incompatible_template()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ConsumableLibrary"),
                _ => Given_is_the_working_directory_PATH("ConsumableLibrary"),
                _ => When_I_create_a_new_component_with_name("PlmComponent", false),
                _ => Then_the_user_was_informed_that_the_template_is_incompatible())
                        .RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_component_without_proj_and_without_codefiles()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_component(false, false),
                _ => Then_the_user_was_informed_that_the_component_was_created_successfully(),
                _ => Then_the_entity_was_created_in_default_namespace("RootComponent")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Error_on_creating_component_with_too_short_name()
        {
            await Runner.AddSteps(
               _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_component_with_name("C", false),
                _ => Then_the_user_was_informed_that_the_name_is_too_short()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Error_on_creating_component_with_too_long_name()
        {
            await Runner.AddSteps(
               _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_component_with_name("VeryLongNameVeryLongNameVeryLongNameVeryLongName" +
                "VeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongName" +
                "VeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongNameVeryLongName" +
                "VeryLongNameVeryLongNameVeryLongName", false),
                _ => Then_the_user_was_informed_that_the_name_is_too_long()
                ).RunAsyncWithTimeout();
        }
    }
}
