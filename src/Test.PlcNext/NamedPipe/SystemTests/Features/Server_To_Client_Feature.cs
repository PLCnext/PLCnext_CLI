#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using PlcNext;
using Test.PlcNext.NamedPipe.SystemTests.StepDefinitions;
using Test.PlcNext.SystemTests.Tools;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Checks all possible scenarios for communication from server to the client.")]
    public class Server_To_Client_Feature : SystemTestBase
    {
        [Scenario(Timeout = 10000)]
        public async Task Propagate_command_result_to_client_true()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_command_successful_finishes("command", "--with", "args"),
                _ => Then_the_client_received_a_command_reply_for_the_command_with_the_result("command --with args", true)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Propagate_command_result_to_client_false()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command  --with args"),
                _ => When_the_command_unsuccessful_finishes("command", "--with", "args"),
                _ => Then_the_client_received_a_command_reply_for_the_command_with_the_result("command  --with args", false)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Cancel_command_does_not_trigger_command_reply()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_cancel_the_command("command --with args"),
                _ => Then_the_client_did_not_receive_a_command_reply_for_the_command("command --with args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Cancel_command_replies_with_cancel()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_cancel_the_command("command --with args"),
                _ => Then_the_the_client_received_a_cancel_reply_for_the_command("command --with args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Cancel_command_replies_with_correct_command_cancel()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_executed_the_command("command --with other,args"),
                _ => When_I_executed_the_command("other command --withFlag"),
                _ => When_I_cancel_the_command("command --with other,args"),
                _ => Then_the_the_client_received_a_cancel_reply_for_the_command("command --with other,args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Reply_to_successful_handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server(),
                _ => When_I_connect_with_a_valid_handshake(),
                _ => Then_the_the_client_received_a_successful_handshake_reply()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Only_one_reply_to_successful_handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server(),
                _ => When_I_connect_with_a_valid_handshake_twice(),
                _ => Then_the_the_client_received_a_handshake_reply_once()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Reply_to_unsuccessful_handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server(),
                _ => When_I_connect_with_a_wrong_handshake(),
                _ => Then_the_the_client_received_an_unsuccessful_handshake_reply()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task No_reply_without_prior_handshake()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_cancel_the_command("command --with args"),
                _ => Then_the_the_client_received_no_reply()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        [ClassData(typeof(MessageDataGenerator))]
        public async Task Propagate_message_MESSAGE_to_client_while_executing_command_message_type_MESSAGETYPE(string message, string messageType)
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_the_command_line_outputs_the_message_MESSAGE_with_the_type_TYPE(message, messageType),
                _ => Then_the_client_received_a_message_with_the_content_CONTENT_and_type_TYPE_for_the_command(message, messageType, "command --with args")
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Show_no_heartbeat_before_any_message_send()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server_with_heartbeat_enabled(),
                _ => When_I_wait_for_TIME_ms(800),
                _ => Then_the_client_received_at_most_COUNT_heartbeats(0)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Show_no_heartbeat_before_any_command_executed()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server_with_heartbeat_enabled_and_valid_handshake(),
                _ => When_I_wait_for_TIME_ms(800),
                _ => Then_the_client_received_at_most_COUNT_heartbeats(0)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Show_heartbeat_after_command_started()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server_with_heartbeat_enabled_and_valid_handshake(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_wait_for_TIME_ms(800),
                _ => Then_the_client_received_at_least_COUNT_heartbeats(4)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Show_heartbeat_until_last_command_finished()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server_with_heartbeat_enabled_and_valid_handshake(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_executed_the_command("command --with other,args"),
                _ => When_I_wait_for_TIME_ms(800),
                _ => When_the_command_successful_finishes("command","--with","args"),
                _ => When_I_wait_for_TIME_ms(600),
                _ => Then_the_client_received_at_least_COUNT_heartbeats(7)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Show_no_heartbeat_after_command_executed()
        {
            await Runner.AddSteps(
                _ => Given_is_a_started_server_with_heartbeat_enabled_and_valid_handshake(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_executed_the_command("command --with other,args"),
                _ => When_I_wait_for_TIME_ms(600),
                _ => When_the_command_successful_finishes("command","--with","args"),
                _ => When_the_command_successful_finishes("command","--with","other,args"),
                _ => When_I_wait_for_TIME_ms(800),
                _ => Then_the_client_received_at_most_COUNT_heartbeats(6)
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Server_registers_client_disconnect()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_the_client_disconnects_from_the_server(),
                _ => Then_the_server_registered_a_disconnect()
            ).RunAsyncWithTimeout();
        }

        [Scenario(Timeout = 10000)]
        public async Task Server_cancels_command_before_suicide()
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => When_I_executed_the_command("command --with args"),
                _ => When_I_kill_the_server(),
                _ => Then_the_the_client_received_a_cancel_reply_for_the_command("command --with args")
            ).RunAsyncWithTimeout();
        }

        public class MessageDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>
            {
                new object[] {"A verbose message.", "verbose"},
                new object[] {"A information message.", "information"},
                new object[] {"A warning message.", "warning"},
                new object[] {"An error message.", "error"},
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public Server_To_Client_Feature(ITestOutputHelper output) : base(output)
        {
            NamedPipeServerFeature serverFeature = new NamedPipeServerFeature();
            if (!serverFeature.FeatureEnabled)
            {
                StepExecution.Current.IgnoreScenario("Disabled named pipe communication");
            }
        }
    }
}
