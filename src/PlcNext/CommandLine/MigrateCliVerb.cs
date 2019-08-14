#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using PlcNext.Common.Commands;

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.MigrateCliVerb, HelpText = "Migrates all caches and user settings from the previous version if available.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class MigrateCliVerb : VerbBase
    {
        protected override Task<int> Execute(ICommandManager commandManager)
        {
            //Implementation in program instead. This is only for the help screen.
            throw new NotImplementedException();
        }
    }
}
