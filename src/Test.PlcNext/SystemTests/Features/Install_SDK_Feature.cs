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
using System.Text;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.StepDefinitions;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Unpack SDK, copy cmake files and update plcncli settings.")]
    public class Install_Sdk_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Unknown_file_format_of_packed_SDK()
        {
            await Runner.AddSteps(
                    _ => Given_is_the_working_directory_PATH("C:/foo/ba"),
                    _ => When_I_install_SDK_to_DESTINATION("Dummy_AXCF2152_Sdk.tar.xy", "C:/foo/ba/xyz"),
                    _ => Then_the_user_was_informed_that_the_file_extension_is_unkown()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Install_SDK_unpacks_SDK()
        {
            await Runner.AddSteps(
                    _ => Given_is_an_empty_workspace(),
                    _ => When_I_install_SDK_to_DESTINATION("Dummy_AXCF2152_Sdk.tar.xz", "C:/foo/ba"),
                    _ => Then_the_sdk_is_available_in_DESTINATION("C:/foo/ba")
                ).RunAsyncWithTimeout();
        }
        
        [Scenario]
        public async Task Delete_All_Files_When_Force_option_is_Used()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => Given_is_that_the_file_exists("C:/foo/ba/notEmpty.txt"),
                _ => When_I_force_install_SDK_to_DESTINATION("Dummy_AXCF2152_Sdk.tar.xz", "C:/foo/ba"),
                _ => Then_the_sdk_is_available_in_DESTINATION("C:/foo/ba"),
                _ => Then_the_file_does_not_exist("C:/foo/ba/notEmpty.txt")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Install_SDK_twice()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => Given_is_that_sdk_exploration_finds_TARGET("AXCF2152", "1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)"),
                _ => When_I_install_SDK_to_DESTINATION("Dummy_AXCF2152_Sdk.tar.xz", "C:/foo/ba"),
                _ => Then_the_user_was_informed_that_the_target_is_already_installed()
                ).RunAsyncWithTimeout();
        }

        public Install_Sdk_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
