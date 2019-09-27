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
    [Verb(CommandLineConstants.SettingVerb, HelpText = "Sets the settings for the command line tool.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class SetSettingsVerb : SetVerb
    {
        [Value(0, MetaName = "key", HelpText = "The key for the changed setting (key must be first string after setting command).", Required = true)]
        public string Key { get; set; }

        [Value(1, MetaName = "value", HelpText = "The value for the changed setting (value must be second string after setting command).", Required = false)]
        public string Value { get; set; }

        [Option('a', CommandLineConstants.AddOption, HelpText = "Indicates that the value should be added to existing values (only for collection keys).", Required = false, SetName = "mode")]
        public bool Add { get; set; }

        [Option('r', CommandLineConstants.RemoveOption, HelpText = "Indicates that the value should be removed from existing values (only for collection keys).", Required = false, SetName = "mode")]
        public bool Remove { get; set; }

        [Option('c', CommandLineConstants.ClearOption, HelpText = "Indicates that the value should be cleared (set to empty).", Required = false, SetName = "mode")]
        public bool Clear { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> SetSettingsUsageExample =>
        new[]
        {
            new UsageExample("Add sdk:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.SettingVerb} {Constants.SdkPathsKey} Path/to/sdk --{CommandLineConstants.AddOption}" ),
            new UsageExample("Replace current list of SDKs with new entry:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.SettingVerb} {Constants.SdkPathsKey} Path/to/sdk " ),
            new UsageExample("Change the attribute prefix to # :", $"{CommandLineConstants.SetVerb} {CommandLineConstants.SettingVerb} {Constants.AttributePrefixKey} #" ),
            new UsageExample("Change the log file path:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.SettingVerb} {Constants.LogFilePathKey} Path/to/log.txt" ),
            new UsageExample("Try to use system commands before using shipped ones:", $"{CommandLineConstants.SetVerb} {CommandLineConstants.SettingVerb} {Constants.UseSystemCommandsKey} True" )
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new SetSettingsCommandArgs(Key, Value, Add, Remove, Clear)))
                                       .ConfigureAwait(false);
        }
    }
}
