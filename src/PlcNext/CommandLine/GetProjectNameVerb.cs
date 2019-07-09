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
    [Verb(CommandLineConstants.ProjectNameVerb, HelpText = "Lists the name of the given project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetProjectNameVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetProjectNameUsageExample =>
            new[]
            {
                new UsageExample("Get name of the project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.ProjectNameVerb} --{CommandLineConstants.PathOption} Path/To/Project ")
            };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(new GetProjectNameCommandArgs(Path));
        }
    }
}
