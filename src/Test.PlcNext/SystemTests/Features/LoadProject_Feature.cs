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
    [FeatureDescription(@"Loads Projects with or without plcnext.proj file.")]
    public class LoadProject_Feature : MockedSystemTestBase
    
    {
        [Scenario]
        public async Task Load_Project_with_same_component_name_in_different_namspaces()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramsWithCorrectComments"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_command_was_executed_without_error()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Load_Project_with_ambiguous_component_comment_in_program()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithAmbiguousComment"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_user_was_informed_that_the_comment_is_ambiguous()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Load_Project_with_nonexisting_component_comment_in_program()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ProgramWithWrongComment"),
                _ => When_I_generate_all_metafiles(),
                _ => Then_the_user_was_informed_that_the_comment_points_to_nonexisting_component()
                ).RunAsyncWithTimeout();
        }

        public LoadProject_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
