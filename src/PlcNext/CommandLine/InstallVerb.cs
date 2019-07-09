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
    [Verb(CommandLineConstants.InstallVerb, HelpText = "Installs the SDK.")]
    [ChildVerbs(typeof(InstallSdkVerb))]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal abstract class InstallVerb : VerbBase
    {
    }
}
