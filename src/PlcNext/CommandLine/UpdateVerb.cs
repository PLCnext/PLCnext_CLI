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
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    [Verb(CommandLineConstants.UpdateVerb, HelpText = "Updates targets.")]
    [ChildVerbs(typeof(UpdateCliVerb), typeof(UpdateProjectTargetsVerb))]
    internal abstract class UpdateVerb : VerbBase
    {
    }
}
