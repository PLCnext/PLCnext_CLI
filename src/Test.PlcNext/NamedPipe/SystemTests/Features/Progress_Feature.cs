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
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.NamedPipe.SystemTests.StepDefinitions;
using Test.PlcNext.SystemTests.Tools;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Check all scenarios with progress.")]
    [IgnoreScenario("Disabled named pipe communication")]
    public class Progress_Feature : SystemTestBase
    {
        [Scenario(Timeout = 10000)]
        public async Task Initial_progress_is_reported_to_the_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_a_new_progress_with_the_maximum_is_started_for_the_command(13, "command", "--with", "args"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille(0)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Progress_is_reported_to_the_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13), "command", "--with", "args"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille(77)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Initial_progress_message_is_reported_to_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_a_new_progress_with_the_maximum_and_the_message_is_started_for_the_command(13, "Initial message", "command", "--with", "args"),
                _ => Then_the_last_reported_progress_has_the_message("Initial message")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Progress_message_is_reported_to_user()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_a_new_progress_with_the_maximum_and_the_message_is_started_for_the_command(13, "Initial message", "command", "--with", "args"),
                _ => When_the_progress_is_reported_for_the_command(new Progress(1, 13, "In between message"), "command", "--with", "args"),
                _ => Then_the_last_reported_progress_has_the_message("In between message")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Incremental_progress_updates_the_progress_and_shows_message()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_the_progress_is_incremented_by_AMOUNT_with_the_message(1,"Increment message"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(154, "Increment message")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Completed_progress_sends_final_progress_message()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_a_new_progress_with_the_maximum_and_the_completed_message_is_started_for_the_command(13, "Completed", "command", "--with", "args"),
                _ => When_the_progress_is_incremented_by_AMOUNT_with_the_message(1,"Increment message"),
                _ => When_all_progresses_are_completed(),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(1000, "Completed")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Infinite_progress_reported_as_information_message()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_an_infinite_progress_with_the_message_is_started("Infinite message"),
                _ => Then_the_client_received_a_message_with_the_content_CONTENT_and_type_TYPE_for_the_command("Infinite message","information","command --with args")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Spawn_infinite_child_progress_reported_as_progress_message_with_unchanged_progress()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_an_infinite_child_progress_with_the_message_is_started("Infinite message"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(77, "Infinite message")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Complete_infinite_child_progress_increments_main_progress()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_an_infinite_child_progress_with_the_message_is_started("Infinite message"),
                _ => When_all_child_progresses_are_completed(),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille(154)
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Spawn_child_progress_reported_as_progress_message_with_unchanged_progress()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_an_child_progress_with_the_maximum_of_MAXIMUM_and_the_message_is_started(7, "Child progress message"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(77, "Child progress message")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Increment_child_progress_updates_progress_and_progress_message()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_an_child_progress_with_the_maximum_of_MAXIMUM_and_the_message_is_started(7, "Infinite message"),
                _ => When_the_progress_is_reported_for_the_command(new Progress(3, 7, "In between child message"), "command", "--with", "args"),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(110, "In between child message")
            ).RunAsyncWithTimeout();
        }
        
        [Scenario(Timeout = 10000)]
        public async Task Complete_child_progress_increments_progress()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_progress_is_started_and_reported_for_the_command(new Progress(1, 13, "fooba"), "command", "--with", "args"),
                _ => When_an_child_progress_with_the_maximum_of_MAXIMUM_and_the_message_is_started(7, "Infinite message"),
                _ => When_all_child_progresses_are_completed(),
                _ => Then_the_last_reported_progress_is_PROGRESS_per_mille(154)
            ).RunAsyncWithTimeout();
        }

        public Progress_Feature(ITestOutputHelper output) : base(output)
        {
        }
    }
}
