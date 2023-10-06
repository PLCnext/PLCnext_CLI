#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
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
    [Verb(CommandLineConstants.TargetsVerb, HelpText = "Lists all targets supported by available SDKs.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class GetTargetsVerb : GetVerb
    {
        [Option(CommandLineConstants.ShortChar, CommandLineConstants.ShortOption, HelpText ="Get a shortened form of the version.")]
        public bool Short { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetTargetsUsageExample =>
        new[]
        {
            new UsageExample("Get all targets from all available SDKs:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.TargetsVerb}"),
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddVerbName(AddDeprecatedInformation(new GetTargetsCommandArgs(string.Empty, Short))))
                                       .ConfigureAwait(false);
        }
    }
}
