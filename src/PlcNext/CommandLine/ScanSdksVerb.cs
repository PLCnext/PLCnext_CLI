#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using PlcNext.Common.Commands;

namespace PlcNext.CommandLine
{
    [Verb("explore-sdks", HelpText = "Forces the CLI to explores all configured SDKs. This will override the cached values of already explored SDKs.",
        Hidden = true)]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class ScanSdksVerb : VerbBase
    {
        protected override Task<int> Execute(ICommandManager commandManager)
        {
            return commandManager.Execute(AddDeprecatedInformation(new ScanSdksCommandArgs()));
        }
    }
}
