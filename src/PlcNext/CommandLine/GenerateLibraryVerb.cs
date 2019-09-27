#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
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
    [DeprecatedVerb("deploy")]
    [Verb(CommandLineConstants.LibraryVerb, Hidden = true, HelpText = "Deprecated. Generates a .pcwlx library.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GenerateLibraryVerb : GenerateVerb
    {
        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Option(CommandLineConstants.MetaPathChar, CommandLineConstants.MetaPathOption, HelpText = "Directory where the library meta files are located.")]
        public string MetaFilesDirectory { get; set; }

        [Option(CommandLineConstants.CompilationChar, CommandLineConstants.CompilationOption, 
            HelpText = "Path to the compilation output file(s), if different from <projectname>/bin.")]
        public string LibraryLocation { get; set; }

        [Option(CommandLineConstants.OutputChar, CommandLineConstants.OutputOption, 
            HelpText = "Output directory for the library. The directory can be either absolute or relative to the project directory.")]
        public string OutputDirectory { get; set; }

        [Option(CommandLineConstants.IdChar, CommandLineConstants.IdOption, HelpText = "The GUID for the library.")]
        public string LibraryGuid { get; set; }

        [Option(CommandLineConstants.TargetChar, CommandLineConstants.TargetOption, HelpText ="List of targets to add to library: <target>[,<version>[,<path to compilation file>]].")]
        public IEnumerable<string> Targets { get; set; }

        [Option('s', EntityKeys.SourceDirectoryKey, HelpText = "The path of the source directories separated by ','. Default is the 'src' directory " +
                                                               "if such a directory exists. If not, the directory " +
                                                               "defined with the '--path' option is used. Relative paths are relative " +
                                                               "to the directory defined with the '--path' option. If any path contains a ' ' quotation " +
                                                               "marks should be used around all paths, e.g. \"path1,path With Space,path2\".",
               Separator = ',')]
        public IEnumerable<string> SourceDirectories { get; set; }

        [Option(CommandLineConstants.ExternalLibrariesChar, CommandLineConstants.ExternalLibrariesOption, HelpText="List of external libraries separated by ',' to add to library." +
                                                                                                                   " For multibinary libraries: specify list of libraries separated by ','" +
                                                                                                                   " which then will be added for every target or specify per " +
                                                                                                                   "target a list of libraries.")]
        public IEnumerable<string> ExternalLibraries { get; set; }
        [Usage]
        public static IEnumerable<UsageExample> GenerateLibraryUsageExample =>
        new[]
        {
            new UsageExample("Generate library for all targets supported by project:"
                , $"{CommandLineConstants.GenerateVerb} {CommandLineConstants.LibraryVerb} --{CommandLineConstants.PathOption} Path/To/Project"),
            new UsageExample("Generate library for targets AXCF2152 and RFC4072S:"
                , $"{CommandLineConstants.GenerateVerb} {CommandLineConstants.LibraryVerb} --{CommandLineConstants.PathOption} Path/To/Project " +
                $"--{CommandLineConstants.TargetOption} AXCF2152 RFC4072S"),
            new UsageExample("Generate library for target with compilation file in special location:"
                , $"{CommandLineConstants.GenerateVerb} {CommandLineConstants.LibraryVerb} --{CommandLineConstants.PathOption} Path/To/Project " +
                $"--{CommandLineConstants.TargetOption} AXCF2152,2019.0,path/to/Project.so"),
            new UsageExample("Generate library with external libraries:"
                , $"{CommandLineConstants.GenerateVerb} {CommandLineConstants.LibraryVerb} --{CommandLineConstants.PathOption} Path/To/Project " +
                $"--{CommandLineConstants.ExternalLibrariesOption} AXCF2152,2019.0,path/to/libforaxc.so,path/to/otherlib.so RFC4072S,path/to/libfornfc.so")
        };

        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(AddDeprecatedInformation(new 
                GenerateLibraryCommandArgs(Path, MetaFilesDirectory, LibraryLocation, OutputDirectory, LibraryGuid, Targets, ExternalLibraries, SourceDirectories)))
                                       .ConfigureAwait(false);
        }
    }
}
