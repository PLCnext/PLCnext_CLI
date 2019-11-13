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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.SDK
{
    internal class CMakeSdkExplorer : ISdkExplorer
    {
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;
        private readonly IProcessManager processManager;
        private readonly IBinariesLocator binariesLocator;
        private readonly ExecutionContext executionContext;
        private readonly IOutputFormatterPool formatterPool;
        private static readonly Regex MakroDefinition = new Regex(@"#define (?<name>\S*)(?<value> \S*)?");
        private readonly HashSet<string> exploredPaths = new HashSet<string>();

        public CMakeSdkExplorer(IFileSystem fileSystem, IEnvironmentService environmentService, IProcessManager processManager, IBinariesLocator binariesLocator, ExecutionContext executionContext, IOutputFormatterPool formatterPool)
        {
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.processManager = processManager;
            this.binariesLocator = binariesLocator;
            this.executionContext = executionContext;
            this.formatterPool = formatterPool;
        }

        public async Task<SdkSchema> ExploreSdk(string sdkRootPath, bool forceExploration = false)
        {
            sdkRootPath = sdkRootPath.CleanPath();

            if (AlreadyExplored())
            {
                return null;
            }

            if (!fileSystem.DirectoryExists(sdkRootPath))
            {
                throw new SdkPathNotExistingException(sdkRootPath);
            }
            VirtualDirectory rootDirectory = fileSystem.GetDirectory(sdkRootPath);
            SdkSchema sdkSchema = await Explore().ConfigureAwait(false);
            executionContext.WriteVerbose($"Finished exploration with the following content:{Environment.NewLine}" +
                                       $"{JsonConvert.SerializeObject(sdkSchema, Formatting.Indented, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore})}");
            exploredPaths.Add(sdkRootPath);

            return sdkSchema;

            async Task<SdkSchema> Explore()
            {
                try
                {
                    executionContext.WriteInformation($"Exploring the SDK {sdkRootPath}.");
                    VirtualDirectory temporaryDirectory = fileSystem.GetTemporaryDirectory();
                    CreateSampleProject(temporaryDirectory);
                    string makeExecutable = FindMakeExecutable();
                    ConfigureSampleProject(temporaryDirectory, makeExecutable);
                    return await ExploreSampleProject(temporaryDirectory, makeExecutable).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new CMakeExplorationException(rootDirectory.FullName, e);
                }
                    
                async Task<SdkSchema> ExploreSampleProject(VirtualDirectory temporaryDirectory, string makeExecutable)
                {
                    VirtualDirectory sourceDirectory = temporaryDirectory;
                    VirtualDirectory binaryDirectory = temporaryDirectory.Directory("cache");

                    using (CMakeConversation conversation = await CMakeConversation.Start(processManager,
                                                                                          binariesLocator,
                                                                                          formatterPool,
                                                                                          temporaryDirectory,
                                                                                          environmentService.Platform == OSPlatform.Windows,
                                                                                          executionContext,
                                                                                          sourceDirectory,
                                                                                          binaryDirectory)
                                                                                   .ConfigureAwait(false))
                    {
                        JArray cache = await conversation.GetCache().ConfigureAwait(false);
                        JArray codeModel = await conversation.GetCodeModel().ConfigureAwait(false);
                        return ExploreCMakeOutput(cache, codeModel);
                    }

                    SdkSchema ExploreCMakeOutput(JArray cache, JArray codeModel)
                    {
                        string[] supportedDevices = GetCacheEntry("ARP_SUPPORTED_DEVICES")["value"].Value<string>()
                                                                                                 .Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);
                        string[] supportedVersions = GetCacheEntry("ARP_SUPPORTED_VERSIONS")["value"].Value<string>()
                                                                                                   .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        IEnumerable<string> includePaths = GetIncludePathsFromCodeModel(out string sysroot);
                        CompilerSpecification specification = GetCompilerSpecs(sysroot);

                        return new SdkSchema
                        {
                            IncludePath = includePaths.ToArray(),
                            CompilerSpec = specification,
                            makepath = makeExecutable,
                            path = rootDirectory.FullName,
                            Target = (from device in supportedDevices
                                      from version in supportedVersions
                                      select new TargetSchema
                                      {
                                          name = device,
                                          version = version
                                      }).ToArray()
                        };

                        CompilerSpecification GetCompilerSpecs(string cmakeSysroot)
                        {
                            string compiler = GetCacheEntry("CMAKE_CXX_COMPILER")["value"].Value<string>();
                            string flags = GetCacheEntry("CMAKE_CXX_FLAGS")["value"].Value<string>();
                            string command = $"--sysroot=\"{cmakeSysroot}\" {flags} -E -P -v -dD a.cxx";
                            StringBuilderUserInterface stringBuilder = new StringBuilderUserInterface(executionContext,true,true,true,true);

                            string setup = GetSetupFile(binaryDirectory);

                            using (IProcess process = processManager.StartProcessWithSetup(compiler.Replace('/', Path.DirectorySeparatorChar),
                                                                                  command, stringBuilder, setup, 
                                                                                  temporaryDirectory.FullName))
                            {
                                process.WaitForExit();
                                if (process.ExitCode != 0)
                                {
                                    throw new FormattableException($"Error while extracting the compiler include paths:{Environment.NewLine}" +
                                                                   $"{stringBuilder.Error}");
                                }
                            }

                            string[] lines = ($"{stringBuilder.Error}{Environment.NewLine}" +
                                              $"{stringBuilder.Information}").Split(new[] {'\n', '\r'},
                                                                                    StringSplitOptions.RemoveEmptyEntries);
                            IEnumerable<string> paths = lines.SkipWhile(l => !l.Contains("#include <...> search starts here:"))
                                                                    .Skip(1)
                                                                    .TakeWhile(l => !l.Contains("End of search list."));
                            return new CompilerSpecification
                            {
                                IncludePath = GetIncludePaths().ToArray(),
                                CompilerMacro = GetMakros().ToArray(),
                                compilerPath = compiler.Replace('/', Path.DirectorySeparatorChar),
                                compilerFlags = flags,
                                compilerSysroot = cmakeSysroot
                            };

                            IEnumerable<MakroDefinition> GetMakros()
                            {
                                return lines.Select(l => MakroDefinition.Match(l))
                                            .Where(l => l.Success)
                                            .Select(m => m.Groups["value"].Success
                                                             ? new MakroDefinition
                                                             {
                                                                 name = m.Groups["name"].Value,
                                                                 value = m.Groups["value"].Value
                                                             }
                                                             : new MakroDefinition {name = m.Groups["name"].Value});
                            }

                            IEnumerable<string> GetIncludePaths()
                            {
                                return paths.Where(p => fileSystem.DirectoryExists(p))
                                            .Select(p => fileSystem.GetDirectory(p).FullName);
                            }
                        }

                        IEnumerable<string> GetIncludePathsFromCodeModel(out string cmakeSysroot)
                        {
                            JObject project = codeModel.OfType<JObject>()
                                                       .Where(o => o.ContainsKey("projects"))
                                                       .SelectMany(o => o["projects"])
                                                       .OfType<JObject>()
                                                       .FirstOrDefault(o => o.ContainsKey("name") &&
                                                                            o["name"].Value<string>() == "MyProject");
                            if (project == null)
                            {
                                throw new FormattableException($"The code model does not contain any project with the name 'MyProject'. " +
                                                               $"The code model contains the following data:{Environment.NewLine}" +
                                                               $"{codeModel.ToString(Formatting.Indented)}");
                            }

                            JObject target = project.ContainsKey("targets")
                                                 ? project["targets"].OfType<JObject>()
                                                                     .Where(o => o.ContainsKey("name"))
                                                                     .FirstOrDefault(o => o["name"].Value<string>() == "MyProject")
                                                 : null;
                            if (target == null)
                            {
                                throw new FormattableException($"The project 'MyProject' does not contain any target with the name 'MyProject'. " +
                                                               $"The project contains the following data:{Environment.NewLine}" +
                                                               $"{project.ToString(Formatting.Indented)}");
                            }

                            cmakeSysroot = target["sysroot"].Value<string>();

                            return target.ContainsKey("fileGroups")
                                       ? target["fileGroups"].OfType<JObject>()
                                                             .Where(o => o.ContainsKey("includePath"))
                                                             .SelectMany(o => o["includePath"])
                                                             .OfType<JObject>()
                                                             .Where(o => o.ContainsKey("path"))
                                                             .Select(o => o["path"].Value<string>())
                                                             .Where(p => fileSystem.DirectoryExists(p))
                                                             .Select(p => fileSystem.GetDirectory(p).FullName)
                                       : Enumerable.Empty<string>();
                        }

                        JObject GetCacheEntry(string key)
                        {
                            JObject entry = cache.OfType<JObject>().FirstOrDefault(t => t.ContainsKey("key") &&
                                                                                        t["key"].Value<string>() == key);
                            if (entry == null)
                            {
                                throw new FormattableException($"The cache entry {key} was expected, but not found. The cache contains the following data:{Environment.NewLine}" +
                                                               $"{cache.ToString(Formatting.Indented)}");
                            }

                            return entry;
                        }
                    }
                }

                void ConfigureSampleProject(VirtualDirectory temporaryDirectory, string makeExecutable)
                {
                    VirtualDirectory cache = temporaryDirectory.Directory("cache");
                    string makeOption = string.IsNullOrEmpty(makeExecutable)
                                            ? string.Empty
                                            : $"-DCMAKE_MAKE_PROGRAM=\"{makeExecutable.Replace('\\', '/')}\" ";
                    string command =
                        $"-DCMAKE_TOOLCHAIN_FILE=\"{rootDirectory.FullName.Replace('\\', '/')}/toolchain.cmake\" " +
                        $"-G \"Unix Makefiles\" " +
                        $"{makeOption}" +
                        $"-DARP_TOOLCHAIN_ROOT=\"{rootDirectory.FullName.Replace('\\', '/')}\" " +
                        $"-DARP_CHECK_DEVICE=OFF " +
                        $"-DARP_CHECK_DEVICE_VERSION=OFF " +
                        $"..";

                    string setup = GetSetupFile(cache);

                    using (IProcess process = processManager.StartProcessWithSetup(binariesLocator.GetExecutableCommand("cmake"),
                                                                          command, executionContext, setup,
                                                                          cache.FullName.Replace("\\", "/"),
                                                                          showOutput: false, showError: false))
                    {
                        process.WaitForExit();
                    }
                }

                void CreateSampleProject(VirtualDirectory temporaryDirectory)
                {
                    string resourceBaseString = "PlcNext.Common.Tools.SDK.SampleProject.";
                    Assembly assembly = Assembly.GetAssembly(typeof(CMakeSdkExplorer));
                    IEnumerable<string> resources = assembly.GetManifestResourceNames()
                                                            .Where(n => n.StartsWith(resourceBaseString, StringComparison.OrdinalIgnoreCase));
                    foreach (string resource in resources)
                    {
                        string fileName = resource.Substring(resourceBaseString.Length);
                        using (Stream fileStream = temporaryDirectory.File(fileName).OpenWrite())
                        using (Stream resourceStream = assembly.GetManifestResourceStream(resource))
                        {
                            resourceStream?.CopyTo(fileStream);
                        }
                    }
                }

                string FindMakeExecutable()
                {
                    if (environmentService.Platform != OSPlatform.Windows)
                    {
                        return string.Empty;
                    }

                    string hintPath = Path.Combine(sdkRootPath, @"sysroots\x86_64-pokysdk-mingw32\usr\bin\make.exe");
                    if (fileSystem.FileExists(hintPath))
                    {
                        return hintPath;
                    }

                    string makePath = rootDirectory.Files("make.exe", true).FirstOrDefault()?.FullName;
                    if (string.IsNullOrEmpty(makePath))
                    {
                        throw new FormattableException($"Could not find the 'make.exe' in the SDK '{sdkRootPath}'. " +
                                                       "This SDK cannot be used, please remove the SDK from the settings.");
                    }

                    return makePath;
                }
            }

            bool AlreadyExplored()
            {
                return !forceExploration && exploredPaths.Contains(sdkRootPath);
            }
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
    }
}
