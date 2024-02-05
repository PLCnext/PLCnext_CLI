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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Project.Persistence;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
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
        private const string PrefixPathOption = "-DCMAKE_PREFIX_PATH=\"%PREFIX_PATH%\"";

        private static Regex IncludeDirectoryPattern = new Regex(@"^(?<name>[^\s]*)_(?<version>\d*\.\d*\.\d*\.\d*)$", RegexOptions.Compiled);

        public CmakeExecuter(IProcessManager processManager, IUserInterface userInterface, IBinariesLocator binariesLocator, 
                             IFileSystem fileSystem, IEnvironmentService environmentService, ExecutionContext executionContext)
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
                    workingDirectory.FullName.Replace("\\", "/", StringComparison.Ordinal),
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

            LoadCmakeOptions();

            bool success = ConfigureCMake();

            return (success, cmakeFolder);

            void GenerateCmakeFile()
            {
                //It is important to get the name first before the cmake file is created in the next line.
                string name = buildInformation.RootEntity.Name;
                VirtualFile cMakeFile = buildInformation.RootFileEntity.Directory.File(Constants.CMakeFileName);
                //TODO Generate cmakefile with template system
                //TODO Set src folders in cmake file (consider foreign project structure)
                CMakeFileGenerator.WriteCMakeFile(cMakeFile, name);
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

            void LoadCmakeOptions()
            {
                if (!string.IsNullOrEmpty(buildInformation.BuildProperties))
                {
                    return;
                }

                if(buildInformation.RootFileEntity?.Directory?.FileExists(Constants.CMakeCommandArgsFileName) == true)
                {
                    string cmakeArgs = string.Empty;
                    VirtualFile commandArgsFile = buildInformation.RootFileEntity.Directory.File(Constants.CMakeCommandArgsFileName);
                    using (Stream fileStream = commandArgsFile.OpenRead())
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            string line = streamReader.ReadLine();
                            if (string.IsNullOrEmpty(line))
                            {
                                continue;
                            }
                            cmakeArgs = string.Join(" ", cmakeArgs, line);
                        }
                    }
                    buildInformation.BuildProperties = cmakeArgs;
                    buildInformation.BuildPropertiesSetByFile = true;
                }
            }

            bool ConfigureCMake()
            {
                executionContext.WriteInformation("Checking if CMake needs to be reconfigured...", showMessagesToUser);
                if ((!cmakeFolder.FileExists("CMakeCache.txt") ||
                     buildInformation.Configure ||
                     !CacheHasValidTimestamp() ||
                     !IsCorrectlyConfigured() ||
                     OutputOptionDiffersFromStagingPrefix()) &&
                    !buildInformation.NoConfigure)
                {
                    string cmakeCommand = GenerateCmakeCommand(buildInformation.Target.Name,
                                                               buildInformation.Target.LongVersion);
                    
                    executionContext.WriteInformation("Configuring CMake...", showMessagesToUser);

                    bool result = CallCmake(cmakeFolder, cmakeCommand, showMessagesToUser, throwOnError, showWarningsToUser);

                    AddTimestamp();

                    return result;
                }

                if (!string.IsNullOrEmpty(buildInformation.BuildProperties) && !buildInformation.BuildPropertiesSetByFile)
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
                    string sdkRoot = buildInformation.SdkInformation.Root.FullName.Replace("\\", "/", StringComparison.Ordinal);
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_TOOLCHAIN_FILE=", StringComparison.Ordinal))
                    {
                        commandParts.Add(ToolchainFileOption.Replace("%SDK_ROOT%", sdkRoot, StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_TOOLCHAIN_ROOT=", StringComparison.Ordinal))
                    {
                        commandParts.Add(ToolchainRootOption.Replace("%SDK_ROOT%", sdkRoot, StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_BUILD_TYPE=", StringComparison.Ordinal))
                    {
                        commandParts.Add(BuildTypeOption.Replace("%BUILD_TYPE%", GetRealBuildType(), StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_DEVICE=", StringComparison.Ordinal))
                    {
                        commandParts.Add(DeviceOption.Replace("%TARGET%", $"\"{target}\"", StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DARP_DEVICE_VERSION=", StringComparison.Ordinal))
                    {
                        commandParts.Add(DeviceVersionOption.Replace("%VERSION%", $"\"{version}\"", StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_STAGING_PREFIX=", StringComparison.Ordinal))
                    {
                        commandParts.Add(StagingPrefixOption.Replace("%STAGING_PREFIX%", GenerateStagingPrefixForTarget(), StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-DCMAKE_PREFIX_PATH=", StringComparison.Ordinal) &&
                        IsIncludePathAvailable(out string includePath))
                    {
                        commandParts.Add(PrefixPathOption.Replace("%PREFIX_PATH%", includePath, StringComparison.Ordinal));
                    }
                    if (!buildInformation.BuildProperties.Contains("-G ", StringComparison.Ordinal))
                    {
                        commandParts.Add(GeneratorOption);
                        if (buildInformation.SdkInformation.MakeFile != null && !buildInformation.BuildProperties.Contains("-DCMAKE_MAKE_PROGRAM ", StringComparison.Ordinal))
                        {
                            commandParts.Add(MakeFileOption.Replace("%MAKE_EXE%", $"\"{buildInformation.SdkInformation.MakeFile.FullName.Replace("\\", "/", StringComparison.Ordinal)}\"", StringComparison.Ordinal));
                        }
                    }
                    if (!string.IsNullOrEmpty(buildInformation.BuildProperties))
                    {
                        commandParts.Add(buildInformation.BuildProperties);
                    }
                    commandParts.Add($"\"{buildInformation.RootFileEntity.Directory.FullName.Replace("\\", "/", StringComparison.Ordinal)}\"");
                    return string.Join(" ", commandParts);

                    string GenerateStagingPrefixForTarget()
                    {
                        string basePath = buildInformation.RootFileEntity.Directory.FullName;
                        return buildInformation.Output != null
                                   ? OutputOptionFullPath()
                                   : Path.Combine(basePath, Constants.LibraryFolderName)
                                         .Replace(Path.DirectorySeparatorChar, '/');
                    }

                    bool IsIncludePathAvailable(out string path)
                    {
                        path = null;
                        if (!buildInformation.RootFileEntity.Directory.DirectoryExists("external"))
                        {
                            return false;
                        }

                        Dictionary<Version, VirtualDirectory> versions = new Dictionary<Version, VirtualDirectory>();
                        VirtualDirectory externalDirectory = buildInformation.RootFileEntity.Directory.Directory("external");
                        foreach (VirtualDirectory directory in externalDirectory.Directories)
                        {
                            Match patternMatch = IncludeDirectoryPattern.Match(directory.Name);
                            if (!patternMatch.Success ||
                                !Version.TryParse(patternMatch.Groups["version"].Value, out Version includeVersion) ||
                                target != patternMatch.Groups["name"].Value)
                            {
                                continue;
                            }

                            versions.Add(includeVersion, directory);
                        }

                        Version actualVersion = Version.Parse(buildInformation.Target.Version);
                        Version bestMatch = versions.Keys.Where(v => v <= actualVersion)
                                                    .OrderByDescending(v => v)
                                                    .FirstOrDefault();
                        if (bestMatch != null)
                        {
                            VirtualDirectory directory = versions[bestMatch];
                            if (directory.DirectoryExists(buildInformation.BuildType))
                            {
                                path = directory.Directory(buildInformation.BuildType).FullName;
                            }
                            else if (directory.DirectoryExists(Constants.ReleaseFolderName))
                            {
                                path = directory.Directory(Constants.ReleaseFolderName).FullName;
                            }
                            else
                            {
                                path = directory.FullName;
                            }
                        }
                        else
                        {
                            path = externalDirectory.FullName;
                        }

                        return true;
                    }
                }

                string OutputOptionFullPath()
                {
                    return fileSystem.GetDirectory(buildInformation.Output,
                                                   buildInformation.RootFileEntity.Directory.FullName)
                                     .FullName.Replace(Path.DirectorySeparatorChar, '/');
                }

                bool OutputOptionDiffersFromStagingPrefix()
                {
                    return buildInformation.Output != null && 
                           !buildInformation.BuildEntity.BuildSystem.InstallationPaths.Any(p => p.StartsWith(OutputOptionFullPath(), StringComparison.Ordinal));
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

                void AddTimestamp()
                {
                    if (buildInformation.RootFileEntity?.Directory?.FileExists(Constants.CMakeCommandArgsFileName) == true)
                    {
                        VirtualFile commandArgsFile = buildInformation.RootFileEntity.Directory.File(Constants.CMakeCommandArgsFileName);
                        VirtualFile timestampFile = cmakeFolder.File(Constants.CMakeTimestampFileName);

                        JObject timestamp = new JObject
                            {
                                new JProperty("FlagsWriteTime", new JValue(commandArgsFile.LastWriteTime))
                            };

                        using (Stream fileStream = timestampFile.OpenWrite())
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            fileStream.SetLength(0);
                            jsonWriter.Formatting = Formatting.Indented;
                            timestamp.WriteTo(jsonWriter);
                        }
                    }
                    else
                    {
                        if (cmakeFolder.FileExists(Constants.CMakeTimestampFileName))
                        {
                            cmakeFolder.File(Constants.CMakeTimestampFileName).Delete();
                        }
                    }
                }

                bool CacheHasValidTimestamp()
                {
                    if(buildInformation.RootFileEntity?.Directory?.FileExists(Constants.CMakeCommandArgsFileName) == true)
                    {
                        DateTime commandArgsLastWriteTime = buildInformation.RootFileEntity
                                                                            .Directory
                                                                            .File(Constants.CMakeCommandArgsFileName)
                                                                            .LastWriteTime;
                        if (cmakeFolder.FileExists(Constants.CMakeTimestampFileName))
                        {
                            VirtualFile timestampFile = cmakeFolder.File(Constants.CMakeTimestampFileName);
                            try
                            {
                                using(Stream fileStream = timestampFile.OpenRead())
                                using (StreamReader reader = new StreamReader(fileStream))
                                using (JsonReader jsonReader = new JsonTextReader(reader))
                                {
                                    JObject fileContent = JObject.Load(jsonReader);
                                    if(fileContent.ContainsKey("FlagsWriteTime") &&
                                        fileContent["FlagsWriteTime"].Type == JTokenType.Date)
                                    {
                                        DateTime savedTimeStamp = fileContent["FlagsWriteTime"].Value<DateTime>();
                                        if(savedTimeStamp.CompareTo(commandArgsLastWriteTime) == 0)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            catch (JsonReaderException)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!cmakeFolder.FileExists(Constants.CMakeTimestampFileName))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
    }
}
