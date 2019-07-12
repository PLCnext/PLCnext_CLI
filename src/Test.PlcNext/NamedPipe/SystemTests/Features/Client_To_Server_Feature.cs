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
using Test.PlcNext.NamedPipe.Tools;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Checks all possible scenarios for communication from client to the server.")]
    [IgnoreScenario("Disabled named pipe communication")]
    public class Client_To_Server_Feature : SystemTestBase
    {
        [Scenario(Timeout = 10000)]
        public async Task Propagate_Commands_To_Command_Line()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => Then_the_command_was_triggered("command", "--with", "args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Do_Not_React_To_Messages_Without_Prior_Handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client_without_handshake(),
                _ => When_I_executed_the_command("command --with args"),
                _ => Then_no_command_was_triggered()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Do_Not_React_To_Messages_With_Prior_Unsuccessful_Handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client_with_wrong_handshake(),
                _ => When_I_executed_the_command("command --with args"),
                _ => Then_no_command_was_triggered()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Kill_Server_On_Command()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_kill_the_server(),
                _ => Then_the_server_disconnected()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Cancel_command_on_cancel_message()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_cancel_the_command("command --with args"),
                _ => Then_the_command_was_canceled("command", "--with", "args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Do_not_cancel_command_on_cancel_message_without_command()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_cancel_the_command(),
                _ => Then_the_command_was_not_canceled("command", "--with", "args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Trigger_command_twice_executes_only_once()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_executed_the_command("command --with args"),
                _ => Then_the_command_was_triggered_once("command", "--with", "args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Trigger_command_twice_with_same_id()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command_twice_with_same_id("command --with args"),
                _ => Then_the_command_was_triggered_once("command", "--with", "args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Cancel_command_cancels_the_correct_command()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_executed_the_command("command --with other,args"),
                _ => When_I_executed_the_command("other command --withFlag"),
                _ => When_I_cancel_the_command("command --with other,args"),
                _ => Then_the_command_was_canceled("command", "--with", "other,args")
            ).RunAsyncWithTimeout();
        }

        public Client_To_Server_Feature(ITestOutputHelper output) : base(output)
        {
        }
    }
}
