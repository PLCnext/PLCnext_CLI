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
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    [Verb(CommandLineConstants.SdkVerb, HelpText = "Installs the SDK.")]

    internal class InstallSdkVerb : InstallVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "The path to the packed SDK (.tar.xz or .sh).", Required = true)]
        public string Sdk { get; set; }

        [Option(CommandLineConstants.DestinationChar, CommandLineConstants.DestinationOption, HelpText = "The destination path, where the SDK should be installed. " +
                                                               "It is necessary to use different destinations for each SDK.", 
            Required = true)]
        public string Destination { get; set; }

        [Option(CommandLineConstants.ForceChar, CommandLineConstants.ForceOption, HelpText = "Overrides existing files with same name.")]
        public bool Force { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> InstallSdkUsageExample =>
        new[]
        {
            new UsageExample("install SDK to destination folder", $"{CommandLineConstants.InstallVerb} {CommandLineConstants.SdkVerb} -{CommandLineConstants.DestinationChar} Path/To/SDK/AXCF2152/2019.0  -{CommandLineConstants.PathChar} path/to/axcf2152.tar.xz")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new InstallSdkCommandArgs(Sdk, Destination, Force)))
                                       .ConfigureAwait(false); 
        }
    }
}