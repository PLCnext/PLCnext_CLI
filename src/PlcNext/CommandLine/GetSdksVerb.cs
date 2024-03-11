﻿#region Copyright
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
    [Verb(CommandLineConstants.SdksVerb, HelpText = "Lists all available SDKs.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class GetSdksVerb : GetVerb
    {
        [Usage]
        public static IEnumerable<UsageExample> GetSdksUsageExample =>
        new[]
        {
            new UsageExample("Get all SDKs:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.SdksVerb}"),
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddVerbName(AddDeprecatedInformation(new GetSdksCommandArgs(string.Empty, true))))
                                       .ConfigureAwait(false);
        }
    }
}
