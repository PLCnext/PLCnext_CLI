#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.NamedPipe.SystemTests.StepDefinitions;
using Xunit;

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.Features
{
    [FeatureDescription(@"Check the get commands using the integrated named pipe server and command line.")]
    public class Totally_Integrated_Get_Feature : CommandLineIntegrationTestBase
    {
        [Scenario]
        [ClassData(typeof(GetCommandDataGenerator))]
        public async Task Executing_the_get_COMMAND_command_with_the_project_returns_the_reply(string command, string project, string reply)
        {
            await Runner.AddSteps(
                _ => Given_is_a_connected_client(),
                _ => Given_is_the_project_with_set_default_path(project),
                _ => When_I_executed_the_command($"get {command}"),
                _ => Then_the_client_received_a_message_as_shown_in_the_file_within_TIMEOUT_s(reply,2)
            ).RunAsyncWithTimeout();
        }
        private class GetCommandDataGenerator : IEnumerable<object[]>
        {
            private readonly List<object[]> data = new List<object[]>
            {
                new object[] {"include-paths", "Standard", "GetIncludePathsReply.json"},
                new object[] {"include-paths", "StandardWithMissingTarget", "GetIncludePathsPartialErrorReply.json"},
                new object[] {"sdks", "Standard", "GetSdksReply.json"},
                new object[] {"project-sdks", "StandardWithMissingTarget", "GetSdksPartialErrorReply.json"},
                new object[] {"targets", "Standard", "GetTargetsReply.json"},
                new object[] {"compiler-specifications", "Standard", "GetCompilerSpecsReply.json"},
                new object[] {"compiler-specifications", "StandardWithMissingTarget", "GetCompilerSpecsPartialErrorReply.json"},
                new object[] {"setting AttributePrefix", "Standard", "GetSingleSettingReply.json"},
                new object[] {"setting -a", "Standard", "GetAllSettingsReply.json"},
                new object[] {"project-information", "Standard", "GetProjectInformationStandardReply.json"},
                new object[] {"project-information", "AppProject", "GetProjectInformationAppProjectReply.json"},
                new object[] {"project-information", "StandardWithMissingTarget", "GetProjectInformationMissingTargetReply.json"},
                new object[] {"project-information -s extern,src", "ComponentsInMultipleSources", "GetProjectInformationMultipleSourcesReply.json"},
                new object[] {"project-information", "AlmostAmbiguous", "GetProjectInformationAlmostAmbiguousReply.json"},
            };

            public IEnumerator<object[]> GetEnumerator() => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}