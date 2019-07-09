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
using PlcNext.Common.Tools;

namespace PlcNext.CommandLine
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    [Verb(CommandLineConstants.ProjectTargetsVerb, HelpText = "Updates the targets of the given project. All targets without matching SDK are updated to the next higher available version.")]
    internal class UpdateProjectTargetsVerb : UpdateVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Path to the project settings file.", Default = "./" + Constants.ProjectFileName)]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> UpdateTargetsUsageExample => 
        new[]
        {
            new UsageExample("If current working directory is project root:", $"{CommandLineConstants.UpdateVerb} {CommandLineConstants.ProjectTargetsVerb}"),
            new UsageExample("Otherwise:", $"{CommandLineConstants.UpdateVerb} {CommandLineConstants.ProjectTargetsVerb} --{CommandLineConstants.PathOption} ./path/to/projectroot")
        };
        

        protected override Task<int> Execute(ICommandManager commandManager)
        {
            return commandManager.Execute(new UpdateTargetsCommandArgs(Path));
        }
    }
}
