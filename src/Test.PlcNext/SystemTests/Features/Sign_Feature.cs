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
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription("User signs library during deploy.")]
    public class Sign_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Sign_library_without_sign_with_PKCS12_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,19.0.0.12345" },
                    BuildType = "Debug",
                    PKCS12 = "path/to/file.p12",
                    NoTimestamp = true
                }),
                _ => Then_the_user_was_informed_that_sign_option_is_missing()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_PKCS12_without_timestamp()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs 
                { 
                    Targets = new[]{ "AXCF2152,19.0" }, 
                    BuildType = "Debug",
                    Sign = true, 
                    PKCS12 = "path/to/file.p12", 
                    NoTimestamp = true
                }),
                _ => Then_the_deploy_was_executed_without_error(),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibrarySignCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_PEM_files_with_timestampserver_in_networkconfig()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,19.0.0.12345" },
                    BuildType = "Debug",
                    Sign = true,
                    PrivateKey = "path/to/file.pem",
                    PublicKey = "path/to/file.pem",
                    Certificates = new[] { "path/to/certificate.pem", "path/with space to/other/certificate.pem" },
                    Timestamp = true
                }),
                _ => Then_the_deploy_was_executed_without_error(),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("DemoLibrarySignCommandArgs2.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_PEM_files_and_pkcs12_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,19.0.0.12345" },
                    BuildType = "Debug",
                    Sign = true,
                    PrivateKey = "path/to/file.pem",
                    PublicKey = "path/to/file.pem",
                    Certificates = new[] { "path/to/certificate.pem", "path/with space to/other/certificate.pem" },
                    PKCS12 = "test.p12",
                    NoTimestamp = true
                }),
                _ => Then_the_user_was_informed_that_sign_options_are_incorrectly_combined()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_PEM_file_and_pkcs12_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,19.0.0.12345" },
                    BuildType = "Debug",
                    Sign = true,
                    PrivateKey = "path/to/file.pem",
                    PKCS12 = "test.p12",
                    NoTimestamp = true
                }),
                _ => Then_the_user_was_informed_that_sign_options_are_incorrectly_combined()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_config_file()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithConfigFile"),
                _ => Given_is_the_working_directory_PATH("ProjectWithConfigFile"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,20.6.0.12345" },
                }),
                _ => Then_the_deploy_was_executed_without_error(),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("ProjectWithConfigFileCommandArgs.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_same_option_in_config_and_command()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithConfigFile"),
                _ => Given_is_the_working_directory_PATH("ProjectWithConfigFile"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,20.6.0.12345" },
                    PrivateKey = "path/to/file.pem"
                }),
                _ => Then_the_deploy_was_executed_without_error(),
                _ => Then_the_library_was_generated_with_the_following_command_arguments("ProjectWithConfigFileCommandArgs2.txt")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_config_file_and_option_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithConfigFile"),
                _ => Given_is_the_working_directory_PATH("ProjectWithConfigFile"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,20.6.0.12345" },
                    Sign = true,
                    PKCS12= "abcd.p12"
                }),
                _ => Then_the_user_was_informed_that_sign_options_are_incorrectly_combined()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_config_file_and_timestamp_option_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProjectWithConfigFile"),
                _ => Given_is_the_working_directory_PATH("ProjectWithConfigFile"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,20.6.0.12345" },
                    NoTimestamp = true
                }),
                _ => Then_the_user_was_informed_that_sign_options_are_incorrectly_combined()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Sign_library_with_PKCS12_without_timestamp_decision()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Demo"),
                _ => Given_is_the_working_directory_PATH("Demo"),
                _ => When_I_deploy(new DeployCommandArgs
                {
                    Targets = new[] { "AXCF2152,19.0.0.12345" },
                    BuildType = "Debug",
                    Sign = true,
                    PKCS12 = "path/to/file.p12"
                }),
                _ => Then_the_user_was_informed_that_a_timestamp_decision_is_mandatory()
                ).RunAsyncWithTimeout();
        }


        public Sign_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
