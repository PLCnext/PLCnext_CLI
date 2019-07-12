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
using System.IO;
using System.Text;
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
    [FeatureDescription(@"Generate the pcwlx library.")]
    public class Generate_Library_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Generate_library_executed()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_specific_id()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs { LibraryId = "3125fbc7-77b1-47c4-b5f9-39872cd6df9c" }),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryCommandArgsWithSpecificId.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_with_malformated_library_id_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs { LibraryId = "3125fbc7xxx" }),
                _ => Then_the_user_was_informed_that_the_library_id_is_malformatted()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_without_library_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutLibrary"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutLibrary"),
                _ => When_I_generate_the_library(),
                _ => Then_the_user_was_informed_that_the_library_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_without_meta_files_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutMeta"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutMeta"),
                _ => When_I_generate_the_library(),
                _ => Then_the_user_was_informed_that_the_meta_files_were_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_different_paths()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs{LibraryLocation = "foo", MetaFileDirectory = Path.Combine("ba","Meta"), OutputDirectory = "out"}),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryDifferentPathsCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_different_builder_path()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_a_different_library_builder_location(),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryCommandArgs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_without_library_builder_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_library_builder_is_missing(),
                _ => When_I_generate_the_library(),
                _ => Then_the_user_was_informed_that_the_library_builder_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_multibinary()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithMultibinary"),
                _ => Given_is_the_working_directory_PATH("DemoWithMultibinary"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345", "AXCF2152,2.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs {Targets = new[] { "axcf2152,1.0", "axcf2152,2"} }),
                _ => Then_the_library_was_generated_with_the_following_content("DemoWithMultibinaryLibraryCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_Targets_with_different_locations()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] {"axcf2152,1.0,foo/AXCF2152_1.0.0.12345/Release" },
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out"
                }),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryDifferentPathsCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_without_compmeta_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithoutCompmeta"),
                _ => Given_is_the_working_directory_PATH("DemoWithoutCompmeta"),
                _ => When_I_generate_the_library(),
                _ => Then_the_user_was_informed_that_a_compmeta_file_was_not_found()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_without_SDK_and_project_file()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_are_settings_with_the_cleared_setting_KEY("SdkPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(),
                _ => Then_the_library_was_generated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_Targets_with_different_locations_withoutSDK()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithDifferentPaths"),
                _ => Given_is_the_working_directory_PATH("DemoWithDifferentPaths"),
                _ => Given_are_settings_with_the_cleared_setting_KEY("SdkPaths"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("axcf2152,1.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,1.0.0.12345,foo/AXCF2152_1.0.0.12345/Release" },
                    MetaFileDirectory = Path.Combine("ba", "Meta"),
                    OutputDirectory = "out"
                }),
                _ => Then_the_library_was_generated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_cmake()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_cmake_build_system_exists_for_targets("AXCF2152,19.0.0.12345"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("intermediate/cmake/AXCF2152,19.0.0.12345/Debug/Some/Path/T,o/Some.so"),
                // if path which is returned by cmake server is relative, it is always relative to cmake build system
                _ => Given_cmake_returns_a_codemodel_with_the_following_content("Demo", "Some/Path/T,o/Some.so"),
                _ => When_I_generate_the_library(),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryWithExternalLibs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/T,o/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    ExternalLibraries = new[] { "\"Some/Path/T,o/Some.so\"" }
                }),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryWithExternalLibs.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_for_targets()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("DemoWithMultibinary"),
                _ => Given_is_the_working_directory_PATH("DemoWithMultibinary"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,1.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,1.0,Some/Path/To/Some.so" }
                }),
                _ => Then_the_library_was_generated_with_the_following_content("DemoLibraryWithExternalLibsMultibinary.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => Given_is_that_the_file_exists("OtherLib.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,19.0,Some/Path/To/Some.so", "OtherLib.so" }
                }),
                _ => Then_the_user_was_informed_that_the_library_option_is_wrong_combined()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_wrong_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "nfc482s,19.0,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_the_library_option_is_malformatted()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_ambiguous_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_the_target_is_ambiguous()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_wrong_version_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,1.0,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_the_library_option_is_malformatted()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_ambiguous_version_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => Given_is_that_the_file_exists("Some/Path/To/Some.so"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,1.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,1,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_the_target_is_ambiguous()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_with_external_libraries_via_option_nonexisting_lib_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => Given_is_that_the_directory_exists("sysroots"),
                _ => When_I_generate_the_library(new LibraryCommandArgs
                {
                    Targets = new[] { "axcf2152,19.0.0.12345", "axcf2152,2.0.0.12345" },
                    ExternalLibraries = new[] { "axcf2152,2,Some/Path/To/Some.so" }
                }),
                _ => Then_the_user_was_informed_that_the_library_was_not_found()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Generate_library_executed_without_cmake_build_system()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_generate_the_library(),
                _ => Then_the_user_was_informed_that_the_cmake_build_system_was_not_found()
                ).RunAsyncWithTimeout();
        }

        public Generate_Library_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
