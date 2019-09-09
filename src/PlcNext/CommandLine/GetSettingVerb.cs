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
    [Verb(CommandLineConstants.SettingVerb, HelpText = "Lists all settings or a specific setting with their value.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetSettingVerb : GetVerb
    {
        [Value(0, MetaName = "key", HelpText = "Get the value for the specified key (key must be first string after setting command).", Required = false)]
        public string Key { get; set; }

        [Option(CommandLineConstants.AllChar, CommandLineConstants.AllOption, HelpText = "Get all available keys with their value.", Required = false)]
        public bool All { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetSettingUsageExample =>
        new[]
        {
            new UsageExample("Get all settings:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.SettingVerb} --{CommandLineConstants.AllOption}"),
            new UsageExample("Get current attribute prefix:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.SettingVerb} {Constants.AttributePrefixKey}"),
            new UsageExample("Get list of SDK paths:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.SettingVerb} {Constants.SdkPathsKey}"),
            new UsageExample("Get path to log file:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.SettingVerb} {Constants.LogFilePathKey}")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new GetSettingsCommandArgs(Key??string.Empty, All)));
        }
    }
}
