#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using CommandLine;

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.SetVerb, HelpText = "Sets things like settings and targets.")]
    [ChildVerbs(typeof(SetSettingsVerb), typeof(SetTargetsVerb))]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal abstract class SetVerb : VerbBase
    {
    }
}
