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
    [Verb(CommandLineConstants.TargetVerb, HelpText = "Sets the target for a project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class SetTargetsVerb : SetVerb
    {
        [Option(CommandLineConstants.NameChar, CommandLineConstants.NameOption, HelpText = "The target to be added or removed.", Required = true)]
        public string Name { get; set; }

        [Option(CommandLineConstants.VersionChar, CommandLineConstants.VersionOption, HelpText = "The version of the target to be added or removed.", Required = false)]
        public string Version { get; set; }

        [Option(CommandLineConstants.AddChar, CommandLineConstants.AddOption, HelpText = "Indicates that the target should be added to existing targets.", Required = false, SetName = "mode")]
        public bool Add { get; set; }

        [Option(CommandLineConstants.RemoveChar, CommandLineConstants.RemoveOption, HelpText = "Indicates that the target should be removed from existing targets.", Required = false, SetName = "mode")]
        public bool Remove { get; set; }

        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.", Required = false)]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> SetTargetUsageExample =>
        new[]
        {
            new UsageExample("Add target to project:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.TargetVerb} --{CommandLineConstants.AddOption} -{CommandLineConstants.PathChar} Path/To/Project -{CommandLineConstants.NameChar} axcf2152 -{CommandLineConstants.VersionChar} 2019.0"),
            new UsageExample("Remove target from project:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.TargetVerb} --{CommandLineConstants.RemoveOption} --{CommandLineConstants.PathOption} Path/To/Project --{CommandLineConstants.NameOption} axcf2152")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddVerbName(AddDeprecatedInformation(new SetTargetsCommandArgs(Name, Version, Add, Remove, Path))))
                                       .ConfigureAwait(false);
        }

    }
}
