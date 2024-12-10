#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.StepDefinitions;
using Test.PlcNext.Tools;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"User executes build via CLI with different parameters.")]
    public class Build_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Build_project_on_build()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_build_the_project(),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_with_metadata_on_build()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithMetadata"),
                _ => When_I_build_the_project(),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_on_build_from_within_project()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_execute_build_from_inside_the_project_folder(),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Error_when_specifying_unsupported_target_on_build()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_build_the_project_for_target("Unsupported Target"),
                _ => Then_the_user_was_informed_that_the_target_was_not_found()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_for_target_when_no_target_is_supported_builds_the_projects()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_build_the_project_for_target("AXCF2152,1.0"),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Error_on_build_when_no_target_is_supported()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithComponent"),
                _ => When_I_build_the_project(),
                _ => Then_the_user_was_informed_that_the_project_does_not_contain_any_target()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_with_build_type_and_architecture()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm"),
                _ => When_I_build_the_project_with_build_type("Release"),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_for_target_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm2"),
                _ => Given_is_the_working_directory_PATH("Arp.Plc.Esm2"),
                _ => When_I_build_the_project_from_inside_the_project_folder_for_target("axcf2152","1.0"),
                _ => Then_the_build_was_executed()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm2"),
                _ => Given_is_the_working_directory_PATH("Arp.Plc.Esm2"),
                _ => When_I_execute_build_from_inside_the_project_folder(),
                _ => Then_the_user_was_informed_that_the_project_does_not_contain_any_target()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_without_cmakefile_generates_file()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Arp.Plc.Esm2"),
                _ => Given_is_the_working_directory_PATH("Arp.Plc.Esm2"),
                _ => When_I_build_the_project_from_inside_the_project_folder_for_target("axcf2152", "1.0"),
                _ => Then_the_cmake_file_was_generated()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_with_cmake_command_args_option()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_build_the_project_with_cmake_args("-this -is a -test"),
                _ => Then_the_cmake_args_are_used(new string[] { "-this", "-is", "a", "-test"})
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Build_project_with_cmake_command_args_file()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("CMakeArgsFile"),
                _ => When_I_build_the_project(),
                _ => Then_the_cmake_args_are_used(new string[] { "-this", "-is", "a", "-test" })
                ).RunAsyncWithTimeout();
        }
    }
}
