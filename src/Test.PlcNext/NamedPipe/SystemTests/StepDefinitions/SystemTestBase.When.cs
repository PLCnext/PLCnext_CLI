#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using PlcNext.NamedPipeServer.Communication;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase
    {
        protected async Task When_I_connect_with_a_valid_handshake()
        {
            await ScenarioContext.SendHandshake(CommunicationConstants.SupportedProtocolVersions
                                                                      .OrderByDescending(v => v)
                                                                      .First());
        }
        protected async Task When_I_connect_with_a_valid_handshake_twice()
        {
            await ScenarioContext.SendHandshake(CommunicationConstants.SupportedProtocolVersions
                                                                      .OrderByDescending(v => v)
                                                                      .First(),
                                                2);
        }

        protected void When_I_connect_without_a_handshake()
        {
            //Nothing to do
        }

        protected async Task When_I_connect_with_a_wrong_handshake()
        {
            await ScenarioContext.SendHandshake(new Version(CommunicationConstants.SupportedProtocolVersions
                                                                                  .OrderBy(v => v)
                                                                                  .First().Major - 1, 0));
        }
        protected async Task When_I_executed_the_command(string command)
        {
            await ScenarioContext.SendCommand(command);
        }

        protected async Task When_I_executed_the_command_twice_with_same_id(string command)
        {
            await ScenarioContext.SendCommand(command, 2);
        }

        protected async Task When_I_kill_the_server()
        {
            await ScenarioContext.SendKillCommand();
        }

        protected async Task When_I_cancel_the_command(string command)
        {
            await ScenarioContext.CancelCommand(command);
        }

        protected async Task When_I_cancel_the_command()
        {
            await ScenarioContext.CancelCommand(null);
        }

        protected void When_the_command_successful_finishes(params string[] command)
        {
            ScenarioContext.FinishCommand(command, true);
        }

        protected void When_the_command_unsuccessful_finishes(params string[] command)
        {
            ScenarioContext.FinishCommand(command, false);
        }

        protected void When_the_command_line_outputs_the_message_MESSAGE_with_the_type_TYPE(string message, string type)
        {
            ScenarioContext.RaiseCommandLineMessage(message, type);
        }

        protected void When_I_wait_for_TIME_ms(int time)
        {
            Thread.Sleep(time);
        }

        protected void When_an_infinite_progress_with_the_message_is_started(string message)
        {
            ScenarioContext.StartInfiniteProgress(message);
        }

        protected void When_an_infinite_child_progress_with_the_message_is_started(string message)
        {
            ScenarioContext.StartInfiniteChildProgress(message);
        }

        protected void When_a_new_progress_with_the_maximum_is_started_for_the_command(int maximum, params string[] command)
        {
            ScenarioContext.StartProgress(maximum, command);
        }

        protected void When_a_new_progress_with_the_maximum_and_the_completed_message_is_started_for_the_command(int maximum, string message, params string[] command)
        {
            ScenarioContext.StartProgress(maximum,command, completedMessage: message);
        }

        protected void When_an_child_progress_with_the_maximum_of_MAXIMUM_and_the_message_is_started(int maximum, string message)
        {
            ScenarioContext.StartChildProgress(maximum, message);
        }

        protected void When_the_progress_is_reported_for_the_command(Progress progress, params string[] command)
        {
            ScenarioContext.ReportProgress(progress, command);
        }

        protected void When_a_new_progress_with_the_maximum_and_the_message_is_started_for_the_command(int maximum, string message, params string[] command)
        {
            ScenarioContext.StartProgress(maximum, command, message);
        }

        protected CompositeStep When_the_progress_is_started_and_reported_for_the_command(Progress progress, params string[] command)
        {
            return CompositeStep.DefineNew()
                                .AddSteps(_ => When_a_new_progress_with_the_maximum_is_started_for_the_command(progress.ProgressMaximum, command),
                                          _ => When_the_progress_is_reported_for_the_command(progress,command))
                                .Build();
        }

        protected void When_the_progress_is_incremented_by_AMOUNT_with_the_message(int amount, string message)
        {
            ScenarioContext.IncrementProgress(amount, message);
        }

        protected void When_all_progresses_are_completed()
        {
            ScenarioContext.CompleteProgress();
        }

        protected void When_all_child_progresses_are_completed()
        {
            ScenarioContext.CompleteChildProgress();
        }

        protected void When_the_client_disconnects_from_the_server()
        {
            ScenarioContext.DisconnectClient();
        }
    }
}
