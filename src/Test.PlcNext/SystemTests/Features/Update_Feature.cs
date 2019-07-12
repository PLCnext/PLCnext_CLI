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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.DotNet.PlatformAbstractions;
using Test.PlcNext.SystemTests.StepDefinitions;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Update the CLI with mocked online services.")]
    public class Update_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Update_CLI_to_knewest_version()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_version_of_the_CLI_is_VERSION(new Version(99, 99, 99, 0))
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_CLI_to_specific_version()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(98, 0, 0)),
                _ => Then_the_version_of_the_CLI_is_VERSION(new Version(98, 0, 0, 0))
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Downgrade_CLI_to_specific_version()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(0, 0, 0)),
                _ => Then_the_version_of_the_CLI_is_VERSION(new Version(0, 0, 0, 0))
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_CLI_migrates_settings_to_knew_version()
        {
            await Runner.AddSteps(
                _ => Given_are_settings_with_the_modification_KEY_VALUE("AttributePrefix", "!"),
                _ => When_I_update_the_CLI_to_the_version(new Version(98, 0, 0)),
                _ => Then_the_new_CLI_setting_has_the_value("AttributePrefix", "!")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Downgrade_CLI_migrates_settings_to_knew_version()
        {
            await Runner.AddSteps(
                _ => Given_are_settings_with_the_modification_KEY_VALUE("AttributePrefix", "!"),
                _ => When_I_update_the_CLI_to_the_version(new Version(0, 0, 0)),
                _ => Then_the_new_CLI_setting_has_the_value("AttributePrefix", "!")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Downgrade_CLI_warns_about_missing_settings()
        {
            await Runner.AddSteps(
                _ => Given_are_settings_with_the_modification_KEY_VALUE("AttributePrefix", "!"),
                _ => When_I_update_the_CLI_to_the_version(new Version(0, 0, 0)),
                _ => Then_the_user_was_informed_that_the_setting_was_lost("SdkPaths")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Repository_file_not_reachable_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_that_the_update_repository_file_is_unreachable(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_user_was_informed_that_the_site_is_not_reachable("http://localhost/cli/repository.xml")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Invalid_proxy_settings_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_latest_version_with_the_proxy("http://1.2.3.4:1234"),
                _ => Then_the_user_was_informed_that_the_site_is_not_reachable("http://localhost/cli/repository.xml")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Security_breach_in_repository_file_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_manipulated_repository_file(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_user_was_informed_that_the_repository_file_was_manipulated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Repository_file_certificate_not_reachable_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_that_the_update_repository_file_certificate_is_unreachable(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_user_was_informed_that_the_site_is_not_reachable("http://localhost/cli/repository.xml.cer")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Security_breach_in_zip_file_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(97, 0, 0)),
                _ => Then_the_user_was_informed_that_the_archive_file_was_manipulated()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Zip_file_unreachable_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(96, 0, 0)),
                _ => Then_the_user_was_informed_that_the_site_is_not_reachable($"http://localhost/cli/96.0.0/linux/x64/plcncli.zip")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_CLI_to_unkown_version_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(1, 2, 3)),
                _ => Then_the_user_was_informed_that_the_version_is_not_available(new Version(1, 2, 3))
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_CLI_when_already_updated_informs_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_repository_file_with_only_older_versions(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_user_was_informed_that_the_CLI_is_up_to_date()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Get_all_available_versions()
        {
            await Runner.AddSteps(
                _ => Given_is_a_repository_file_with_only_older_versions(),
                _ => When_I_get_all_available_CLI_versions(),
                _ => Then_the_user_was_informed_that_the_versions_are_available(new Version(0, 0, 0))
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Unkown_file_format_of_setup()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_to_the_version(new Version(95, 0, 0)),
                _ => Then_the_user_was_informed_that_the_file_extension_is_unkown()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task PublicKeyFileMissing()
        {
            await Runner.AddSteps(
                _ => Given_is_that_the_public_key_for_the_cli_repository_is_missing(),
                _ => When_I_update_the_CLI_to_the_latest_version(),
                _ => Then_the_user_was_informed_that_the_cli_public_key_is_missing()
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_With_file()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_update_the_CLI_with_the_file("plcncli_linux_x64_98_0_0.zip"),
                _ => Then_the_version_of_the_CLI_is_VERSION(new Version(98, 0, 0, 0))
            ).RunAsyncWithTimeout();
        }

        public Update_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
