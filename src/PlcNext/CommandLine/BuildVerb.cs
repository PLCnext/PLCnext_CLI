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

namespace PlcNext.CommandLine
{
    [Verb(CommandLineConstants.BuildVerb, HelpText = "Builds the project.")]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class BuildVerb : VerbBase
    {
        private string output;

        [Option(CommandLineConstants.PathChar, CommandLineConstants.PathOption, HelpText = "Directory where the plcnext.proj file is located.")]
        public string Path { get; set; }

        [Option(CommandLineConstants.TargetChar, CommandLineConstants.TargetOption, HelpText ="Targets for which the build shall be executed. " +
                                                    "Targets should have the form <target>[,<version>] [...]. " +
                                                    "The complete version can be but must not be used. " +
                                                    "The combination of target and version must be unambiguous. " +
                                                    "If the version contains a space, quotation marks around that target are necessary. " +
                                                    "WARNING: Do not use this option directly before the cmake options, " +
                                                    "because the cmake options will be interpreted as targets.")]
        public IEnumerable<string> Targets { get; set; }

        [Option(CommandLineConstants.BuildtypeChar, CommandLineConstants.BuildtypeOption, HelpText = "Build type for the build, e.g. Release")]
        public string BuildType { get; set; }

        [Option(CommandLineConstants.ConfigureOption, HelpText = "Force CMake to make a new configuration", SetName = "Configure")]
        public bool Configure { get; set; }

        [Option(CommandLineConstants.NoconfigureOption, HelpText = "Force CMake to do only a build", SetName = "NoConfigure")]
        public bool NoConfigure { get; set; }

        [Option(CommandLineConstants.OutputChar, CommandLineConstants.OutputOption, HelpText = "Sets the directory in which CMake is executed. " +
                                                     "The directory can be either absolute or relative to the project directory. " +
                                                     "To set the directory in which the library is installed, " +
                                                     "set the CMake option '-DCMAKE_STAGING_PREFIX'. " +
                                                     "If the build will be executed for multiple targets, " +
                                                     "the specified directory is used as the base directory with " +
                                                     "subdirectories for each target.")]
        public string Output
        {
            get => output;
            set
            {
                output = value;
                OutputSpecified = true;
            }
        }

        public bool OutputSpecified { get; set; }

        [Value(0, MetaName = "cmake options", HelpText= "Additional CMake options for the build. " +
                                          "They will only be applied when the build is configured. " +
                                          "To reconfigure the build use the '--configure' option. " +
                                          "It is necessary to set these options with a leading ' -- '. " +
                                          "Quotation marks will not be preserved. To add quotation marks " +
                                          "to the options, use the escape string '%22' instead. NO other " +
                                          "escape sequences such as '%20' will be unescaped.")]
        public IEnumerable<string> BuildProperties { get; set; }

        [Usage]
        public static IEnumerable<UsageExample> GenerateLibraryUsageExample =>
        new[]
        {
            new UsageExample("build for all targets supported by project:", $"{CommandLineConstants.BuildVerb} --{CommandLineConstants.PathOption} Path/To/Project"),
            new UsageExample("build project for target AXCF2152:", $"{CommandLineConstants.BuildVerb} --{CommandLineConstants.PathOption} Path/To/Project -{CommandLineConstants.TargetChar} AXCF2152"),
            new UsageExample("build project with additional options:",
                $"{CommandLineConstants.BuildVerb} --{CommandLineConstants.PathOption} Path/To/Project -{CommandLineConstants.TargetChar} AXCF2152 --{CommandLineConstants.ConfigureOption} -- -G %22Unix Makefiles%22 -DCMAKE_MAKE_PROGRAM=%22mymakepath%22"),
            new UsageExample("build with a specifal folder for library and CMake files:", $"{CommandLineConstants.BuildVerb} --{CommandLineConstants.OutputOption} build/cmake -- -DCMAKE_STAGING_PREFIX=Path/To/Project/build/install"),
        };
        
        protected override async Task<int> Execute(ICommandManager commandManager)
        {
            return await commandManager.Execute(new BuildCommandArgs(Path, Targets, BuildType, Configure, NoConfigure,
                                                                     BuildProperties, OutputSpecified ? Output : null));
        }
    }
}
