#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
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
using static PlcNext.CommandLine.CommandLineConstants;

namespace PlcNext.CommandLine
{
    [Verb(AppComponentsVerb, HelpText = "Lists all appcomponents of a project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetAppComponentsVerb : GetVerb
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

        [Usage]
        public static IEnumerable<UsageExample> GetAppComponentsUsageExample =>
        new[]
        {
            new UsageExample("Get all appcomponents of project:", $"{CommandLineConstants.GetVerb} {AppComponentsVerb} --{PathOption} Path/To/Project ")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(new GetAppComponentsCommandArgs(Path, SourceDirectories));
        }
    }
}
