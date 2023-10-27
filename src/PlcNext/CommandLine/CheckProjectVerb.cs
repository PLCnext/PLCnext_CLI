#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using CommandLine;
using PlcNext.Common.Commands;
using System.Reflection;
using System.Threading.Tasks;
using static PlcNext.CommandLine.CommandLineConstants;

namespace PlcNext.CommandLine
{
    [Verb("check-project", HelpText = "Checks the given project for allowed version.",
        Hidden = true)]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class CheckProjectVerb : VerbBase
    {
        [Option(PathChar, PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        protected override Task<int> Execute(ICommandManager commandManager)
        {
            return commandManager.Execute(AddVerbName(AddDeprecatedInformation(new CheckProjectCommandArgs(Path))));
        }
    }
}
