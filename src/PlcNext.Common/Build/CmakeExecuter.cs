#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Project;
using PlcNext.Common.Project.Persistence;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Build
{
    internal class CmakeExecuter : IBuildExecuter
    {
        private readonly IProcessManager processManager;
        private readonly IUserInterface userInterface;
        private readonly IBinariesLocator binariesLocator;
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;
        private readonly ExecutionContext executionContext;
        private readonly IOutputFormatterPool formatterPool;

        private const string ToolchainFileOption = "-DCMAKE_TOOLCHAIN_FILE=\"%SDK_ROOT%/toolchain.cmake\"";
        private const string GeneratorOption = "-G \"Unix Makefiles\"";
        private const string BuildTypeOption = "-DCMAKE_BUILD_TYPE=%BUILD_TYPE%";
        private const string DeviceOption = "-DARP_DEVICE=%TARGET%";
        private const string DeviceVersionOption = "-DARP_DEVICE_VERSION=%VERSION%";
        private const string ToolchainRootOption = "-DARP_TOOLCHAIN_ROOT=\"%SDK_ROOT%\"";
        private const string StagingPrefixOption = "-DCMAKE_STAGING_PREFIX=\"%STAGING_PREFIX%\"";
        private const string MakeFileOption = "-DCMAKE_MAKE_PROGRAM=%MAKE_EXE%";

        public CmakeExecuter(IProcessManager processManager, IUserInterface userInterface, IBinariesLocator binariesLocator, IFileSystem fileSystem, IEnvironmentService environmentService, ExecutionContext executionContext, IOutputFormatterPool formatterPool)
        {
            this.processManager = processManager;
            this.userInterface = userInterface;
            this.binariesLocator = binariesLocator;
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.executionContext = executionContext;
            this.formatterPool = formatterPool;
        }

        public void ExecuteBuild(BuildInformation buildInformation, ChangeObservable observable)
        {
            if (string.IsNullOrEmpty(binariesLocator.GetExecutableCommand("cmake")))
            {
                throw new FormattableException("CMake cannot be found. Please install a new version of CMake and ensure that it will be added to the search path settings.");
            }

            (bool _, VirtualDirectory cmakeFolder) = EnsureConfigured(buildInformation, observable, true);

            TouchMainCMakeFile();
            CallCmake(cmakeFolder, "--build . --target install", true, true);

            void TouchMainCMakeFile()
            {
                if (buildInformation.RootProjectEntity.Version > new Version(1, 0))
                {
                    //do not touch newer project version cmake file
                    return;
                }
                if (!buildInformation.RootFileEntity.Directory.FileExists("CMakeLists.txt"))
                {
                    throw new CMakeFileNotFoundException();
                }
                VirtualFile cmakeFile = buildInformation.RootFileEntity.Directory.File("CMakeLists.txt");
                cmakeFile.Touch();
            }
        }

        private bool CallCmake(VirtualDirectory workingDirectory, string command, bool showOutput, bool throwOnError)
        {
            return CallCmake(workingDirectory, command, showOutput, throwOnError, showOutput);
        }

        private bool CallCmake(VirtualDirectory workingDirectory, string command, bool showOutput, bool throwOnError,
            bool showWarnings)
        {
            string setup = GetSetupFile(workingDirectory);
            
            using (IProcess process = processManager.StartProcessWithSetup(binariesLocator.GetExecutableCommand("cmake"),
                    command, userInterface, setup,
                    workingDirectory.FullName.Replace("\\", "/"),
                    showOutput: showOutput, showError: showWarnings))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    if (throwOnError)
                    {
                        throw new FormattableException("cmake process exited with error");
                    }

                    return false;
                }
            }

            return true;
        }

        private string GetSetupFile(VirtualDirectory scriptDirectory)
        {
            string environmentSetupScript = environmentService.Platform == OSPlatform.Linux
                ? $"{Constants.EnvironmentSetupScriptFile}.{Constants.ScriptFileExtensionLinux}"
                : $"{Constants.EnvironmentSetupScriptFile}.{Constants.ScriptFileExtensionWin}";

            if (scriptDirectory.CheckDirectlyFileExists(environmentSetupScript))
            {
                //return environmentSetupScript;
                return Path.Combine(scriptDirectory.FullName, environmentSetupScript);
            }
            return null;
        }

        public (bool, VirtualDirectory) EnsureConfigured(BuildInformation buildInformation, ChangeObservable observable = null, bool throwOnError = false,
                                                         bool showMessagesToUser = true)
        {
            return EnsureConfigured(buildInformation, showMessagesToUser, observable, throwOnError, showMessagesToUser);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Build", "CA1031:Modify 'EnsureConfigured' to catch a more specific exception type, or rethrow the exception.", Justification = "Exception is purposefully logged and ignored.")]
        public (bool, VirtualDirectory) EnsureConfigured(BuildInformation buildInformation, bool showWarningsToUser, ChangeObservable observable = null,
                                                         bool throwOnError = false, bool showMessagesToUser = true)
        {
            if (!CmakeFileExists())
            {
                GenerateCmakeFile();
            }

            VirtualDirectory cmakeFolder = CreateCmakeFolder();

            bool success = ConfigureCMake();

            return (success, cmakeFolder);

            void GenerateCmakeFile()
            {
                VirtualFile cMakeFile = buildInformation.RootFileEntity.Directory.File(Constants.CMakeFileName);
                //TODO Generate cmakefile with template system
                //TODO Set src folders in cmake file (consider foreign project structure)
                CMakeFileGenerator.WriteCMakeFile(cMakeFile, buildInformation.RootEntity.Name);
                observable?.OnNext(new Change(() => { /*Do not delete, because user need to make changes perhaps*/ }, $"Generated cmake file {cMakeFile.FullName}"));
            }

            bool CmakeFileExists()
            {
                return buildInformation.RootFileEntity.Directory.FileExists(Constants.CMakeFileName);
            }

            VirtualDirectory CreateCmakeFolder()
            {
                
                VirtualDirectory result = buildInformation.BuildEntity.BuildSystemDirectory;
                if (buildInformation.Configure && !buildInformation.NoConfigure)
                {
                    result.Clear();
                    observable?.OnNext(new Change(() => result.UnClear(), $"Cleared cmake directory."));
                }
                return result;
            }

            string GetRealBuildType()
            {
                string buildType = !string.IsNullOrEmpty(buildInformation.BuildType)
                                       ? buildInformation.BuildType
                                       : Constants.ReleaseFolderName;
                return buildType;
            }

            bool ConfigureCMake()
            {
                executionContext.WriteInformation("Checking if CMake needs to be reconfigured...", showMessagesToUser);
                if ((!cmakeFolder.FileExists("CMakeCache.txt") ||
                     buildInformation.Configure ||
                     !IsCorrectlyConfigured()) &&
                    !buildInformation.NoConfigure)
                {
                    string cmakeCommand = GenerateCmakeCommand(buildInformation.Target.Name,
                                                               buildInformation.Target.LongVersion);

                    if (showMessagesToUser)
                    {
                        executionContext.WriteInformation("Configuring CMake...");
                    }
                    return CallCmake(cmakeFolder, cmakeCommand, showMessagesToUser, throwOnError, showWarningsToUser);
                }

                if (!string.IsNullOrEmpty(buildInformation.BuildProperties))
                {
                    executionContext.WriteWarning($"The specified build options will not be used, " +
                                                  $"because no reconfiguration is necessary. " +
                                                  $"To force a reconfiguration please use the '--configure' command option.",
                                                  showMessagesToUser);
                }

                return true;

                string GenerateCmakeCommand(string target, string version)
                {
                    List<string> commandParts = new List<string>();
                    string sdkRoot = buildInformation.SdkInformation.Root.FullName.Replace("\\", "/");
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_TOOLCHAIN_FILE "))
                    {
                        commandParts.Add(ToolchainFileOption.Replace("%SDK_ROOT%", sdkRoot));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_TOOLCHAIN_ROOT "))
                    {
                        commandParts.Add(ToolchainRootOption.Replace("%SDK_ROOT%", sdkRoot));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_BUILD_TYPE "))
                    {
                        commandParts.Add(BuildTypeOption.Replace("%BUILD_TYPE%", GetRealBuildType()));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_DEVICE "))
                    {
                        commandParts.Add(DeviceOption.Replace("%TARGET%", $"\"{target}\""));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_DEVICE_VERSION "))
                    {
                        commandParts.Add(DeviceVersionOption.Replace("%VERSION%", $"\"{version}\""));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_STAGING_PREFIX "))
                    {
                        commandParts.Add(StagingPrefixOption.Replace("%STAGING_PREFIX%", GenerateStagingPrefixForTarget()));
                    }
                    if (!buildInformation.BuildProperties.Contains("-G "))
                    {
                        commandParts.Add(GeneratorOption);
                        if (buildInformation.SdkInformation.MakeFile != null && !buildInformation.BuildProperties.Contains("-DCMAKE_MAKE_PROGRAM "))
                        {
                            commandParts.Add(MakeFileOption.Replace("%MAKE_EXE%", $"\"{buildInformation.SdkInformation.MakeFile.FullName.Replace("\\", "/")}\""));
                        }
                    }
                    if (!string.IsNullOrEmpty(buildInformation.BuildProperties))
                    {
                        commandParts.Add(buildInformation.BuildProperties);
                    }
                    commandParts.Add($"\"{buildInformation.RootFileEntity.Directory.FullName.Replace("\\", "/")}\"");
                    return string.Join(" ", commandParts);

                    string GenerateStagingPrefixForTarget()
                    {
                        string basePath = buildInformation.RootFileEntity.Directory.FullName;
                        return buildInformation.Output != null
                                   ? fileSystem.GetDirectory(buildInformation.Output,
                                                             buildInformation.RootFileEntity.Directory.FullName)
                                               .FullName
                                   : Path.Combine(basePath, Constants.LibraryFolderName)
                                         .Replace(Path.DirectorySeparatorChar, '/');
                    }
                }

                bool IsCorrectlyConfigured()
                {
                    try
                    {
                        return buildInformation.BuildEntity.HasBuildSystem &&
                               buildInformation.BuildEntity.BuildSystem != null;
                    }
                    catch (Exception e)
                    {
                        if (!IsTimeout(e))
                        {
                            executionContext.WriteVerbose($"The project is not correctly configured:{Environment.NewLine}{e}");
                            return false;
                        }
                    }

                    return true; //this is a timeout so we dont know if it is correctly configured

                    bool IsTimeout(Exception exception)
                    {
                        return exception is TimeoutException ||
                               exception is AggregateException aggregate &&
                               aggregate.InnerExceptions.Any(e => e is TimeoutException);
                    }
                }
            }
        }
    }
}
