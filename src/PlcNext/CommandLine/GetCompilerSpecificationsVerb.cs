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
    [Verb(CommandLineConstants.CompilerSpecificationsVerb, HelpText = "Lists the compiler specifications - such as the default include path, macros,... - for all used compilers.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetCompilerSpecificationsVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetCompilerSpecificationsUsageExample =>
            new[]
            {
                new UsageExample("Get specifications of all compilers:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.CompilerSpecificationsVerb} --{CommandLineConstants.PathOption} Path/To/Project ")
            };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new GetCompilerSpecificationsCommandArgs(Path)));
        }
    }
}
