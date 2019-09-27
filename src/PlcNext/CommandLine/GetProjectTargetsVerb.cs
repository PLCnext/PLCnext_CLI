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
    [DeprecatedVerb("get project-information")]
    [Verb(CommandLineConstants.ProjectTargetsVerb, Hidden = true, HelpText = "Deprecated. Use 'get project-information' instead. Lists all targets supported by the given project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetProjectTargetsVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Option(CommandLineConstants.ShortChar, CommandLineConstants.ShortOption, HelpText = "Get a shortened form of the version.")]
        public bool Short { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetTargetsUsageExample =>
            new[]
            {
                new UsageExample("Get targets of project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.ProjectTargetsVerb} --{CommandLineConstants.PathOption} Path/To/Project ")
            };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new GetTargetsCommandArgs(Path, false, Short)))
                                       .ConfigureAwait(false);
        }
    }
}
