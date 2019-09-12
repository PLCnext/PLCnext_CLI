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
    [Verb(CommandLineConstants.GenerateVerb, HelpText = "Generates config and code files.")]
    [ChildVerbs (typeof(GenerateLibraryVerb))]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal abstract class GenerateVerb : VerbBase
    {
    }
}
