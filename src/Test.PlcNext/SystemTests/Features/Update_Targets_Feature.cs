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
    [FeatureDescription(@"Update project-targets if sdk for target is missing.")]
    public class Update_Targets_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Update_project_targets()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithMissingTarget2"),
                _ => When_I_update_the_project_targets(),
                _ => Then_the_project_supports_the_targets("AXCF2152,2.0 LTS (2.0.0.12345 branches/release/2.0.0/ beta)")
            ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_project_targets_without_existing_higher_target_throws_error()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithMissingTarget3"),
                _ => When_I_update_the_project_targets(),
                _ => Then_the_user_was_informed_that_the_target_can_not_be_updated()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Update_project_targets_with_downgrade()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithMissingTarget3"),
                _ => When_I_update_the_project_targets_with_downgrade_option(),
                _ => Then_the_project_supports_the_targets("AXCF2152,1.0 LTS (1.0.0.12345 branches/release/1.0.0/ beta)")
                ).RunAsyncWithTimeout();
        }
    }
}
