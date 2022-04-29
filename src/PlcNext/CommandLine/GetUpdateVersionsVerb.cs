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
    [Verb("update-versions", HelpText = "Lists all available versions to update/downgrade the program.", Hidden = true)]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetUpdateVersionsVerb : GetVerb
    {
        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddVerbName(AddDeprecatedInformation(new GetUpdateVersionsCommandArgs())))
                                       .ConfigureAwait(false);
        }
    }
}
