#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using static PlcNext.CommandLine.CommandLineConstants;

namespace PlcNext.CommandLine
{
    [Verb(ProjectInformationVerb, HelpText = "Lists project information like name, targets, ...")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetProjectInformationVerb : GetVerb
    {
        [Option(PathChar, PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Option('s', EntityKeys.SourceDirectoryKey, HelpText = "The path of the source directories separated by ','. Default is the 'src' directory " +
                                                               "if such a directory exists. If not, the directory " +
                                                               "defined with the '--path' option is used. Relative paths are relative " +
                                                               "to the directory defined with the '--path' option. If any path contains a ' ' quotation " +
                                                               "marks should be used around all paths, e.g. \"path1,path With Space,path2\".",
            Separator = ',')]
        public IEnumerable<string> SourceDirectories { get; set; }

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(new GetProjectInformationCommandArgs(Path, SourceDirectories));
        }
    }
}
