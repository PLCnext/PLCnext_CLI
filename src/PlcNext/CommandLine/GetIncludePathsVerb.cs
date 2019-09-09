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
    [Verb(CommandLineConstants.IncludepathsVerb, HelpText = "Deprecated. Use 'get project-information' instead. Lists all include paths which are relevant for this project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetIncludePathsVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetIncludesUsageExample =>
        new[]
        {
            new UsageExample("Get all include paths of project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.IncludepathsVerb} --{CommandLineConstants.PathOption} Path/To/Project ")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new GetIncludePathsCommandArgs(Path)));
        }
    }
}
