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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.StepDefinitions;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Deploy for the project.")]
    public class Deploy_Feature : MockedSystemTestBase
    {
        public Deploy_Feature(ITestOutputHelper helper) : base(helper)
        {
        }

        [Scenario]
        public async Task Deploy_for_project_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs{BuildType = "Debug"}),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_specific_id_generates_library_with_id()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs { Id = "3125fbc7-77b1-47c4-b5f9-39872cd6df9c", BuildType = "Debug" }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryCommandArgsWithSpecificId.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_custom_namespace_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("PartiallyBuildProjectWithCustomNamespace"),
                _ => Given_is_the_working_directory_PATH("PartiallyBuildProjectWithCustomNamespace"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.17548"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("PrjN_ST_Update_Proj_Targets_2237"),
                _ => When_I_deploy(new DeployCommandArgs { BuildType = "Release" }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("PartiallyBuildProjectWithCustomNamespaceLibraryCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_malformated_library_id_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs { Id = "3125fbc7xxx", BuildType = "Debug" }),
                _ => Then_the_user_was_informed_that_the_library_id_is_malformatted()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_library_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutLibrary"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutLibrary"),
                _ => When_I_deploy(new DeployCommandArgs()),
                _ => Then_the_user_was_informed_that_the_library_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_meta_files_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutMeta"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutMeta"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    BuildType = "Debug",
                    Files = new[] { "bin/libDemo.so|.|axcf2152" },
                }),
                _ => Then_the_user_was_informed_that_the_meta_files_were_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_different_paths_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    LibraryLocation = "foo",
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out",
                    BuildType = "Debug"
                }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryDifferentPathsCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_different_builder_path_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_a_different_library_builder_location(),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs{ BuildType = "Debug" }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_library_builder_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_library_builder_is_missing(),
                _ => When_I_deploy(new DeployCommandArgs{ BuildType = "Debug" }),
                _ => Then_the_user_was_informed_that_the_library_builder_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_multibinary_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithMultibinary"),
                _ => Given_is_the_working_directory_PATH("DemoWithMultibinary"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,100.0.0.12345", "AXCF2152,200.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs { Targets = new[] { "axcf2152,100.0", "axcf2152,200" } }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoWithMultibinaryLibraryCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_targets_with_different_locations_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,1.0" },
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out",
                    Files = new []{ "foo/AXCF2152_1.0.0.12345/Release/libDemo.so|.|axcf2152,1.0" },
                    BuildType = "Debug"
                }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryDifferentPathsCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_compmeta_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutCompmeta"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutCompmeta"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    BuildType = "Debug",
                    Files = new[] { "bin/libDemo.so|.|axcf2152" },
                }),
                _ => Then_the_user_was_informed_that_a_compmeta_file_was_not_found()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_SDK_and_project_file_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_are_settings_with_the_cleared_setting_KEY("SdkPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs{ BuildType = "Debug" }),
                _ => Then_the_library_was_generated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_Targets_with_different_locations_withoutSDK_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_are_settings_with_the_cleared_setting_KEY("SdkPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("axcf2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,1.0.0.12345" },
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out",
                    Files = new[] { "foo/AXCF2152_1.0.0.12345/Release/libDemo.so|.|axcf2152,1.0.0.12345" },
                    BuildType = "Debug"
                }),
                _ => Then_the_library_was_generated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_cmake_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,20.6.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,19.0.0.12345/Debug/Some/Path/T,o/Some.so"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,20.6.0.12345/Debug/Some/Path/T,o/Some.so"),
                // if path which is returned by cmake server is relative, it is always relative to cmake build system
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo", "Some/Path/T,o/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs{ BuildType = "Debug" }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryWithExternalLibs.txt"),
                _ => Then_the_user_was_informed_that_the_library_must_be_transferred_manually("Some.so")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_cmake_with_excluded_library_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("ACXF2152,20.6.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,20.6.0.12345/Release/My/Cust,om/Lib/rary.so"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,20.6.0.12345/Release/My/Other/Library.so"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,20.6.0.12345/Release/Some/Path/T,o/Some.so"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("StandardNew", "My/Cust,om/Lib/rary.so", "My/Other/Library.so", "Some/Path/T,o/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs { BuildType= "Release", ExcludedFiles = new[] { "Library.so", "rary.so" } }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("StandardLibraryWithExternalLib.txt"),
                _ => Then_the_libmeta_file_of_project_NAME_is_generated_with_the_dependencies("Standard","rary.so", "Library.so", "Some.so")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/T,o/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    ExternalLibraries = new[] { "\"Some/Path/T,o/Some.so\"" },
                    BuildType = "Debug"
                }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryWithExternalLibsForAllTargets.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        [IgnoreScenario("TODO as soon as old generate library command is gone, reactivate")]
        public async Task Inform_user_of_target_format_mismatch()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_are_settings_with_the_cleared_setting_KEY("SdkPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("axcf2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,1.0.0.12345,foo/AXCF2152_1.0.0.12345/Release/libDemo.so" },
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out",
                    Files = new[] { "foo/AXCF2152_1.0.0.12345/Release/libDemo.so|.|axcf2152,1.0.0.12345" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_target_is_wrongly_formatted()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_without_target_informs_user_that_target_is_necessary()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithMultibinary"),
                _ => Given_is_the_working_directory_PATH("DemoWithMultibinary"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    ExternalLibraries = new[] { "axcf2152,1.0,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_no_target_is_available()
            ).RunAsyncWithTimeout();
        }

        //TODO update help / requirement
        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_for_targets_generates_library()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithMultibinary"),
                _ => Given_is_the_working_directory_PATH("DemoWithMultibinary"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,100.0.0.12345", "axcf2152,200.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,100.0,Some/Path/To/Some.so" }
                }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibraryWithExternalLibsMultibinary.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_wrong_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "nfc482s,19.0,Some/Path/To/Some.so" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_target_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_ambiguous_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,Some/Path/To/Some.so" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_target_is_ambiguous()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_wrong_version_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,1.0,Some/Path/To/Some.so" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_target_version_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_ambiguous_version_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,1.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,1,Some/Path/To/Some.so" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_target_is_ambiguous()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_external_libraries_via_option_nonexisting_lib_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,2,Some/Path/To/Some.so" },
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_deployment_file_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_with_wrong_wildcard_path_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_code_model_with_the_following_libraries("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    LibraryLocation = "foo",
                    MetaFileDirectory = Path.Combine("ba", "Meta2"),
                    OutputDirectory = "out",
                    BuildType = "Debug"
                }),
                _ => Then_the_user_was_informed_that_the_deployment_file_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_project_without_cmake_build_system_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs{ BuildType = "Debug" }),
                _ => Then_the_user_was_informed_that_the_cmake_build_system_was_not_found()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_acfproject_deploys_the_acfconfig()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_deploy(new DeployCommandArgs { Targets = new[] { "axcf2152,19.0" } }),
                _ => Then_the_acfconfig_file_is_deployed_with_component_into_path(
                    "AcfProject", "AcfProjectComponent", "bin/AXCF2152_19.0.0.12345/Release")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_for_acfproject_deploys_how_to_deploy_file()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_acfproject_with_name("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_deploy(new DeployCommandArgs { Targets = new[] { "axcf2152,19.0" } }),
                _ => Then_the_files_exist_in_location("bin/AXCF2152_19.0.0.12345/Release/How_to_deploy.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_files_deploys_the_files_in_correct_location()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0" },
                    Files = new[] { "MySettings.txt|deploy/settings", "MySettings.txt|." }
                }),
                _ => Then_the_files_exist_in_location(new Dictionary<string, string>
                    {
                        { "bin/AXCF2152_19.0.0.12345/Release/deploy/settings/MySettings.txt",
                            "Here are some settings..."
                        },
                        {
                            "bin/AXCF2152_19.0.0.12345/Release/MySettings.txt",
                            "Here are some settings..."
                        }
                    })
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_files_deploys_the_files_in_correct_location_with_build_type()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0" },
                    Files = new[] { "MySettings.txt|deploy/settings", "MySettings.txt|." },
                    BuildType = "Debug"
                }),
                _ => Then_the_files_exist_in_location(new Dictionary<string, string>
                    {
                        { "bin/AXCF2152_19.0.0.12345/Debug/deploy/settings/MySettings.txt",
                            "Here are some settings..."
                        },
                        {
                            "bin/AXCF2152_19.0.0.12345/Debug/MySettings.txt",
                            "Here are some settings..."
                        }
                    })
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_files_for_missing_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("AcfProject"),
                _ => Given_is_the_working_directory_PATH("AcfProject"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0" },
                    Files = new[] { "MySettings.txt|deploy/settings|axcf2152,2.0" }
                }),
                _ => Then_the_user_was_informed_that_the_target_version_was_not_found()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_when_librarybuilder_exits_with_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_the_librarybuilder_exits_with_error(),
                _ => When_I_deploy(new DeployCommandArgs { BuildType = "Debug" }),
                _ => Then_the_user_was_informed_that_the_library_generation_failed()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_with_library_version_and_description()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_deploy(
                    new DeployCommandArgs 
                    {
                        LibraryDescription = "This is an example description, where something needs to be escaped.",
                        LibraryVersion = "1.2.3.xy",
                        EngineerVersion = "2021.3.1"
                    }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("StandardLibraryWithVersionAndDescription.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_with_library_version_and_description_saved_in_project_file()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_deploy(new DeployCommandArgs()),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("StandardLibraryWithSavedVersionAndDescription.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_with_EngineerVersion()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_deploy(new DeployCommandArgs {  }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("StandardLibraryWithEngineerVersion.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_with_SolutionVersion()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_deploy(new DeployCommandArgs { SolutionVersion = "1.5.0.0" }),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("StandardLibraryWithSolutionVersion.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Deploy_with_EngineerVersion_and_SolutionVersion()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_deploy(new DeployCommandArgs { EngineerVersion="1.2.3.4", SolutionVersion="5.6.7.8"}),
                _ => Then_the_user_was_informed_that_the_deploy_options_are_wrong_combined()
                ).RunAsyncWithTimeout();
        }
    }

    public class DeployCommandArgs
    {

        public string LibraryLocation { get; internal set; }

        public string MetaFileDirectory { get; internal set; }

        public string OutputDirectory { get; internal set; }

        public string Id { get; internal set; }
        public string BuildType { get; internal set; }

        public IEnumerable<string> Targets { get; internal set; }

        public IEnumerable<string> ExternalLibraries { get; internal set; }

        public IEnumerable<string> Files { get; internal set; }

        public string LibraryDescription { get; internal set; }

        public string LibraryVersion { get; internal set; }

        public string EngineerVersion { get; internal set; }

        public string SolutionVersion { get; internal set; }

        public IEnumerable<string> ExcludedFiles { get; internal set; }
    }
}
