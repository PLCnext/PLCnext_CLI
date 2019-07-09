#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

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

        protected void Then_the_command_was_canceled(params string[] command)
        {
            ScenarioContext.CheckCommandCanceled(command, true);
        }

        protected void Then_the_command_was_not_canceled(params string[] command)
        {
            ScenarioContext.CheckCommandCanceled(command, false);
        }

        protected void Then_the_client_received_a_command_reply_for_the_command_with_the_result(
            string command, bool result)
        {
            ScenarioContext.CheckCommandReply(command,result);
        }

        protected void Then_the_client_did_not_receive_a_command_reply_for_the_command(string command)
        {
            ScenarioContext.CheckNoCommandReply(command);
        }

        protected void Then_the_the_client_received_a_cancel_reply_for_the_command(string command)
        {
            ScenarioContext.CheckCancelReply(command);
        }

        protected void Then_the_the_client_received_a_successful_handshake_reply()
        {
            ScenarioContext.CheckHandshakeReply(true);
        }

        protected void Then_the_the_client_received_a_handshake_reply_once()
        {
            ScenarioContext.CountHandshakeReplies(1);
        }

        protected void Then_the_the_client_received_an_unsuccessful_handshake_reply()
        {
            ScenarioContext.CheckHandshakeReply(false);
        }

        protected void Then_the_the_client_received_no_reply()
        {
            ScenarioContext.CheckNoMessage();
        }

        protected void Then_the_client_received_a_message_with_the_content_CONTENT_and_type_TYPE_for_the_command(string content, string type, string command)
        {
            ScenarioContext.CheckMessageReceived(content, type, command);
        }

        protected void Then_the_client_received_at_least_COUNT_heartbeats(int count)
        {
            ScenarioContext.CheckHeartbeatMessages(count);
        }

        protected void Then_the_client_received_at_most_COUNT_heartbeats(int count)
        {
            ScenarioContext.CheckHeartbeatMessages(count, false);
        }

        protected void Then_the_last_reported_progress_is_PROGRESS_per_mille(int progress)
        {
            ScenarioContext.CheckLastReportedProgress(progress);
        }

        protected void Then_the_last_reported_progress_is_PROGRESS_per_mille_with_the_message(int progress, string message)
        {
            ScenarioContext.CheckLastReportedProgress(progress, message);
        }

        protected void Then_the_last_reported_progress_has_the_message(string message)
        {
            ScenarioContext.CheckLastReportedProgress(progressMessage: message);
        }

        protected void Then_the_client_received_a_message_as_shown_in_the_file_within_TIMEOUT_s(string file, int timeout)
        {
            ScenarioContext.CheckMessageIsSameAsFile(file, timeout*1000);
        }

        protected void Then_the_client_received_a_setting_updated_message()
        {
            ScenarioContext.CheckUpdateMessage(UpdateMessageType.Setting);
        }

        protected void Then_the_client_received_a_project_target_updated_message_for_the_project(string project)
        {
            ScenarioContext.CheckUpdateMessage(UpdateMessageType.ProjectSettings, project);
        }

        protected void Then_the_client_received_a_sdks_updated_message()
        {
            ScenarioContext.CheckUpdateMessage(UpdateMessageType.Sdk);
        }

        protected void Then_the_client_received_no_setting_updated_message()
        {
            ScenarioContext.CheckNoMessage();
        }

        protected void Then_the_client_received_no_project_target_updated_message_for_the_project(string project)
        {
            ScenarioContext.CheckNoMessage();
        }

        protected void Then_the_client_received_no_sdks_updated_message()
        {
            ScenarioContext.CheckNoMessage();
        }

        protected void Then_the_server_registered_a_disconnect()
        {
            ScenarioContext.CheckSeverRegisteredDisconnect();
        }
    }
}
