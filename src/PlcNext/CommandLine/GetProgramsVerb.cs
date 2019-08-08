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

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.ProgramsVerb, HelpText = "Deprecated. Use 'get project-information' instead. Lists all programs of a project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GetProgramsVerb : GetVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Option('s', EntityKeys.SourceDirectoryKey, HelpText = "The path of the source directories separated by ','. Default is the 'src' directory " +
                                                               "if such a directory exists. If not, the directory " +
                                                               "defined with the '--path' option is used. Relative paths are relative " +
                                                               "to the directory defined with the '--path' option. If any path contains a ' ' quotation " +
                                                               "marks should be used around all paths, e.g. \"path1,path With Space,path2\".",
            Separator = ',')]
        public IEnumerable<string> SourceDirectories { get; set; }

        [Option(CommandLineConstants.ComponentChar, CommandLineConstants.ComponentOption, HelpText = "Component for which the programs shall be returned.")]
        public string Component { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GetProgramsUsageExample =>
        new[]
        {
            new UsageExample("Get all programs of project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.ProgramsVerb} --{CommandLineConstants.PathOption} Path/To/Project "),
            new UsageExample("Get programs belonging to component comp of project:", $"{CommandLineConstants.GetVerb} {CommandLineConstants.ProgramsVerb} --{CommandLineConstants.PathOption} Path/To/Project --{CommandLineConstants.ComponentOption} comp")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(new GetProgramsCommandArgs(Path, Component, SourceDirectories));
        }
    }
}
