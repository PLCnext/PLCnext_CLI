#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using PlcNext.Common.Commands;

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.ProjectSdksVerb, HelpText = "Lists all available SDKs (for a given project).")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class GetProjectSdksVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetSdksUsageExample =>
        new[]
        {
            new UsageExample("Get SDKs of project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.ProjectSdksVerb} --{CommandLineConstants.PathOption} Path/To/Project ")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddVerbName(AddDeprecatedInformation(new GetSdksCommandArgs(Path, false))))
                                       .ConfigureAwait(false);
        }
    }
}
