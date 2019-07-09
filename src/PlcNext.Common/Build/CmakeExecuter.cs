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

        private const string ToolchainFileOption = "-DCMAKE_TOOLCHAIN_FILE=\"%SDK_ROOT%/toolchain.cmake\"";
        private const string GeneratorOption = "-G \"Unix Makefiles\"";
        private const string BuildTypeOption = "-DCMAKE_BUILD_TYPE=%BUILD_TYPE%";
        private const string DeviceOption = "-DARP_DEVICE=%TARGET%";
        private const string DeviceVersionOption = "-DARP_DEVICE_VERSION=%VERSION%";
        private const string ToolchainRootOption = "-DARP_TOOLCHAIN_ROOT=\"%SDK_ROOT%\"";
        private const string StagingPrefixOption = "-DCMAKE_STAGING_PREFIX=\"%STAGING_PREFIX%\"";
        private const string MakeFileOption = "-DCMAKE_MAKE_PROGRAM=%MAKE_EXE%";

        public CmakeExecuter(IProcessManager processManager, IUserInterface userInterface, IBinariesLocator binariesLocator, IFileSystem fileSystem, IEnvironmentService environmentService, ExecutionContext executionContext)
        {
            this.processManager = processManager;
            this.userInterface = userInterface;
            this.binariesLocator = binariesLocator;
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.executionContext = executionContext;
        }

        public void ExecuteBuild(BuildInformation buildInformation, ChangeObservable observable)
        {
            if (string.IsNullOrEmpty(binariesLocator.GetExecutableCommand("cmake")))
            {
                throw new FormattableException("CMake cannot be found. Please install a new version of CMake and ensure that it will be added to the search path settings.");
            }

            if (!CmakeFileExists())
            {
                GenerateCmakeFile();
            }

            VirtualDirectory cmakeFolder = CreateCmakeFolder();
            
            ConfigureCMake();
            TouchMainCMakeFile();
            CallCmake(cmakeFolder, "--build . --target install", true, true);

            bool CmakeFileExists()
            {
                return buildInformation.RootFileEntity.Directory.FileExists(Constants.CMakeFileName);
            }

            void GenerateCmakeFile()
            {
                VirtualFile CMakeFile = buildInformation.RootFileEntity.Directory.File(Constants.CMakeFileName);
                //TODO Generate cmakefile with template system
                //TODO Set src folders in cmake file (consider foreign project structure)
                CMakeFileGenerator.WriteCMakeFile(CMakeFile, buildInformation.RootEntity.Name);
                observable.OnNext(new Change(() => { /*Do not delete, because user need to make changes perhaps*/ }, $"Generated cmake file {CMakeFile.FullName}"));
            }

            VirtualDirectory CreateCmakeFolder()
            {
                VirtualDirectory result = buildInformation.Output != null
                                              ? buildInformation.MultipleTargets
                                                    ? fileSystem.GetDirectory(buildInformation.Output,
                                                                              buildInformation.RootFileEntity.Directory.FullName)
                                                                .Directory(buildInformation.Target.GetFullName())
                                                                .Directory(GetRealBuildType())
                                                    : fileSystem.GetDirectory(buildInformation.Output,
                                                                              buildInformation.RootFileEntity.Directory.FullName)
                                              : buildInformation.RootFileEntity.Directory
                                                                .Directory(Constants.IntermediateFolderName)
                                                                .Directory(Constants.CmakeFolderName)
                                                                .Directory(buildInformation.Target.GetFullName())
                                                                .Directory(GetRealBuildType());
                if (buildInformation.Configure && !buildInformation.NoConfigure)
                {
                    result.Clear();
                    observable.OnNext(new Change(() => result.UnClear(), $"Cleared cmake directory."));
                }
                return result;
            }

            void CallCmake(VirtualDirectory workingDirectory, string command, bool showOutput, bool checkForError)
            {
                using (IProcess process = processManager.StartProcess(binariesLocator.GetExecutableCommand("cmake"), command, userInterface,
                                                                      workingDirectory.FullName.Replace("\\", "/"),
                                                                      showOutput: showOutput, showError: showOutput))
                {
                    process.WaitForExit();
                    if (process.ExitCode != 0 && checkForError)
                    {
                        throw new FormattableException("cmake process exited with error");
                    }
                }
            }

            string GenerateCmakeCommand(string target, string version)
            {
                List<string> commandParts = new List<string>();
                string sdkRoot = buildInformation.Sdk.Root.FullName.Replace("\\", "/");
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
                    if (buildInformation.Sdk.MakeFile != null && !buildInformation.BuildProperties.Contains("-DCMAKE_MAKE_PROGRAM "))
                    {
                        commandParts.Add(MakeFileOption.Replace("%MAKE_EXE%", $"\"{buildInformation.Sdk.MakeFile.FullName.Replace("\\", "/")}\""));
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
                    //return Path.Combine(basePath, Constants.LibraryFolderName, $"{target}_{version}", GetRealBuildType()).Replace(Path.DirectorySeparatorChar,'/');
                    return Path.Combine(basePath, Constants.LibraryFolderName).Replace(Path.DirectorySeparatorChar, '/');
                }
            }

            string GetRealBuildType()
            {
                string buildType = !string.IsNullOrEmpty(buildInformation.BuildType)
                                       ? buildInformation.BuildType
                                       : Constants.ReleaseFolderName;
                return buildType;
            }

            void ConfigureCMake()
            {
                userInterface.WriteInformation("Checking if CMake needs to be reconfigured...");
                if ((!cmakeFolder.FileExists("CMakeCache.txt") ||
                     buildInformation.Configure ||
                     !IsCorrectlyConfigured()) &&
                    !buildInformation.NoConfigure)
                {
                    string cmakeCommand = GenerateCmakeCommand(buildInformation.Target.Name,
                                                               buildInformation.Target.LongVersion);

                    CallCmake(cmakeFolder, cmakeCommand, true, true);
                }
                else if (!string.IsNullOrEmpty(buildInformation.BuildProperties))
                {
                    userInterface.WriteWarning($"The specified build options will not be used, " +
                                               $"because no reconfiguration is necessary. " +
                                               $"To force a reconfiguration please use the '--configure' command option.");
                }

                bool IsCorrectlyConfigured()
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            Task<CMakeConversation> task = CMakeConversation.Start(processManager,
                                                               binariesLocator,
                                                               fileSystem.GetTemporaryDirectory(),
                                                               environmentService.Platform == OSPlatform.Windows,
                                                               executionContext,
                                                               buildInformation.RootFileEntity.Directory,
                                                               cmakeFolder);
                            task.Wait();
                            task.Result.Dispose();
                            return true;
                        }
                        catch (Exception e)
                        {
                            if (!IsTimeout(e))
                            {
                                executionContext.WriteVerbose($"The project is not correctly configured:{Environment.NewLine}{e}");
                                return false;
                            }
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

            void TouchMainCMakeFile()
            {
                if (!buildInformation.RootFileEntity.Directory.FileExists("CMakeLists.txt"))
                {
                    throw new CMakeFileNotFoundException();
                }
                VirtualFile cmakeFile = buildInformation.RootFileEntity.Directory.File("CMakeLists.txt");
                cmakeFile.Touch();
            }
        }
    }
}
