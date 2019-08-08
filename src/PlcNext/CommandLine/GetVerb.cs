#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using CommandLine;

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.GetVerb, HelpText = "Returns information about projects / targets / ...")]
    [ChildVerbs(typeof(GetComponentsVerb), typeof(GetProgramsVerb), typeof(GetTargetsVerb), typeof(GetProjectTargetsVerb),
        typeof(GetSdksVerb), typeof(GetProjectSdksVerb), typeof(GetSettingVerb),
        typeof(GetUpdateVersionsVerb), typeof(GetIncludePathsVerb), typeof(GetCompilerSpecificationsVerb),
        typeof(GetProjectInformationVerb))]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal abstract class GetVerb : VerbBase
    {
    }
}
