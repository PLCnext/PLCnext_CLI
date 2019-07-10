#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;
using Test.PlcNext.NamedPipe.Tools;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase
    {
        protected void Then_the_command_was_triggered(params string[] command)
        {
            ScenarioContext.CheckCommandTriggered(command);
        }

        protected void Then_the_command_was_triggered_once(params string[] command)
        {
            ScenarioContext.CheckCommandTriggered(command, 1);
        }

        protected void Then_no_command_was_triggered()
        {
            ScenarioContext.CheckCommandTriggered(new string[0]);
        }

        protected void Then_the_server_disconnected()
        {
            ScenarioContext.CheckServerDisconnected();
        }

        protected async Task Then_the_command_was_canceled(params string[] command)
        {
            await ScenarioContext.CheckCommandCanceled(command, true);
        }

        protected async Task Then_the_command_was_not_canceled(params string[] command)
        {
            await ScenarioContext.CheckCommandCanceled(command, false);
        }

        protected async Task Then_the_client_received_a_command_reply_for_the_command_with_the_result(
            string command, bool result)
        {
            await ScenarioContext.CheckCommandReply(command,result);
        }

        protected async Task Then_the_client_did_not_receive_a_command_reply_for_the_command(string command)
        {
            await ScenarioContext.CheckNoCommandReply(command);
        }

        protected async Task Then_the_the_client_received_a_cancel_reply_for_the_command(string command)
        {
            await ScenarioContext.CheckCancelReply(command);
        }

        protected async Task Then_the_the_client_received_a_successful_handshake_reply()
        {
            await ScenarioContext.CheckHandshakeReply(true);
        }

        protected async Task Then_the_the_client_received_a_handshake_reply_once()
        {
            await ScenarioContext.CountHandshakeReplies(1);
        }

        protected async Task Then_the_the_client_received_an_unsuccessful_handshake_reply()
        {
            await ScenarioContext.CheckHandshakeReply(false);
        }

        protected async Task Then_the_the_client_received_no_reply()
        {
            await ScenarioContext.CheckNoMessage();
        }

        protected async Task Then_the_client_received_a_message_with_the_content_CONTENT_and_type_TYPE_for_the_command(string content, string type, string command)
        {
            await ScenarioContext.CheckMessageReceived(content, type, command);
        }

        protected void Then_the_client_received_at_least_COUNT_heartbeats(int count)
        {
            ScenarioContext.CheckHeartbeatMessages(count);
        }

        protected void Then_the_client_received_at_most_COUNT_heartbeats(int count)
        {
            ScenarioContext.CheckHeartbeatMessages(count, false);
        }

        protected async Task Then_the_last_reported_progress_is_PROGRESS_per_mille(int progress)
        {
            await ScenarioContext.CheckLastReportedProgress(progress);
        }

        protected async Task Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(int progress, string message)
        {
            await ScenarioContext.CheckLastReportedProgress(progress, message);
        }

        protected async Task Then_the_last_reported_progress_has_the_message(string message)
        {
            await ScenarioContext.CheckLastReportedProgress(progressMessage: message);
        }

        protected async Task Then_the_client_received_a_message_as_shown_in_the_file_within_TIMEOUT_s(string file, int timeout)
        {
            await ScenarioContext.CheckMessageIsSameAsFile(file, timeout*1000);
        }

        protected async Task Then_the_client_received_a_setting_updated_message()
        {
            await ScenarioContext.CheckUpdateMessage(UpdateMessageType.Setting);
        }

        protected async Task Then_the_client_received_a_project_target_updated_message_for_the_project(string project)
        {
            await ScenarioContext.CheckUpdateMessage(UpdateMessageType.ProjectSettings, project);
        }

        protected async Task Then_the_client_received_a_sdks_updated_message()
        {
            await ScenarioContext.CheckUpdateMessage(UpdateMessageType.Sdk);
        }

        protected async Task Then_the_client_received_no_setting_updated_message()
        {
            await ScenarioContext.CheckNoMessage();
        }

        protected async Task Then_the_client_received_no_project_target_updated_message_for_the_project(string project)
        {
            await ScenarioContext.CheckNoMessage();
        }

        protected async Task Then_the_client_received_no_sdks_updated_message()
        {
            await ScenarioContext.CheckNoMessage();
        }

        protected void Then_the_server_registered_a_disconnect()
        {
            ScenarioContext.CheckSeverRegisteredDisconnect();
        }
    }
}
