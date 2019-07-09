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
                ).RunAsync();
        }

        [Scenario]
        public async Task Install_SDK_unpacks_SDK()
        {
            await Runner.AddSteps(
                    _ => Given_is_an_empty_workspace(),
                    _ => When_I_install_SDK_to_DESTINATION("Dummy_AXCF2152_Sdk.tar.xz", "C:/foo/ba"),
                    _ => Then_the_sdk_is_available_in_DESTINATION("C:/foo/ba")
                ).RunAsync();
        }
    }
}
