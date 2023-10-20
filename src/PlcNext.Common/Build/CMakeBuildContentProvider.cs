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
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Build
{
    internal class CMakeBuildContentProvider : PriorityContentProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly Regex projectNameFinder = new Regex(@"(?<!\S)project\((?<name>[^\s\)]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex LibrariesDecoder = new Regex("(?<path>\\\"[^\\\"]+\\\"|[^ |\\\"]+)", RegexOptions.Compiled);
        private static readonly Regex LibrariesRPathDecoder = new Regex("-rpath(?:-link)?,(?<rpath>(?:\\\"[^\\\"]+\\\"|[^ |\\\"]+;?)*)", RegexOptions.Compiled);
        private readonly ICMakeConversation cmakeConversation;
        private readonly ExecutionContext executionContext;
        private static readonly string[] StaticLibraryFileEndings = new[]
        {
            ".a",
            ".lib"
        };

        public CMakeBuildContentProvider(IFileSystem fileSystem, ICMakeConversation cmakeConversation, ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.cmakeConversation = cmakeConversation;
            this.executionContext = executionContext;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return (owner.IsRoot() && key == EntityKeys.NameKey && GetCMakeFile(owner) != null) ||
                   key == EntityKeys.BuildTypeKey ||
                (key == EntityKeys.InternalBuildSystemKey &&
                GetBuildSystemDirectory(owner) != null) ||
                (key == EntityKeys.InternalExternalLibrariesKey &&
                 owner.HasValue<JObject>() &&
                 owner.Type.Equals(EntityKeys.InternalBuildSystemKey, StringComparison.OrdinalIgnoreCase)) ||
                (key == EntityKeys.InternalInstallationPathsKey &&
                 owner.HasValue<JObject>() &&
                 owner.Type.Equals(EntityKeys.InternalBuildSystemKey, StringComparison.OrdinalIgnoreCase)) ||
                   key == EntityKeys.InternalBuildSystemDirectoryKey;
        }

        private VirtualDirectory GetBuildSystemDirectory(Entity owner, bool validateExist = true)
        {
            TargetEntity targetEntity = TargetEntity.Decorate(owner);
            if (!targetEntity.HasFullName)
            {
                return validateExist
                           ? null
                           : throw new InvalidOperationException("Need to use a target entity as base.");
            }
            string buildType = GetBuildType(owner);
            string binaryDirectory = Path.Combine(owner.Root.Path, Constants.IntermediateFolderName,
                                                  Constants.CmakeFolderName,
                                                  targetEntity.FullName,
                                                  buildType);

            if (!fileSystem.DirectoryExists(binaryDirectory))
            {
                string autoCmakeFolderPath = Path.Combine(owner.Root.Path, Constants.IntermediateFolderName,
                                                          Constants.CmakeAutoBuildFolderName,
                                                          targetEntity.FullName,
                                                          buildType);
                if (CommandEntity.Decorate(owner.Origin).CommandName != "build")
                {
                    binaryDirectory = autoCmakeFolderPath;
                }
                else if(fileSystem.DirectoryExists(autoCmakeFolderPath))
                {
                    fileSystem.GetDirectory(autoCmakeFolderPath, createNew:false).Clear();
                }
            }

            if (!fileSystem.DirectoryExists(binaryDirectory) && validateExist)
            {
                return null;
            }

            return fileSystem.GetDirectory(binaryDirectory);
        }

        private VirtualFile GetCMakeFile(Entity owner)
        {
            if (owner.HasPath)
            {
                string basePath = owner.Path;
                if (fileSystem.FileExists(Constants.CMakeFileName, basePath))
                {
                    return fileSystem.GetFile(Constants.CMakeFileName, basePath);
                }
            }

            return null;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.BuildTypeKey:
                    string buildType = GetBuildType(owner);
                    return owner.Create(key, buildType);
                case EntityKeys.InternalBuildSystemDirectoryKey:
                    VirtualDirectory directory = GetBuildSystemDirectory(owner, false);
                    return owner.Create(key, directory);
                case EntityKeys.InternalBuildSystemKey:
                    VirtualDirectory buildSystemDirectory = GetBuildSystemDirectory(owner);
                    JObject codeModel = GetCodeModel(owner, owner.Root.Name, buildSystemDirectory);
                    return owner.Create(key, codeModel, buildSystemDirectory);
                case EntityKeys.InternalExternalLibrariesKey:
                   IEnumerable<string> externalLibraries = FindExternalLibrariesInCodeModel(owner.Value<JObject>(),
                        owner.Root.Name,
                        owner.Value<VirtualDirectory>());
                    return owner.Create(key, externalLibraries.Select(l => owner.Create(key, l)));
                case EntityKeys.InternalInstallationPathsKey:
                    IEnumerable<string> installationPaths = FindInstallationPaths(owner.Value<JObject>());
                    return owner.Create(key, installationPaths.Select(p => owner.Create(key, p)));
                default:
                    VirtualFile cmakeFile = GetCMakeFile(owner);
                    using (Stream fileStream = cmakeFile.OpenRead())
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line))
                            {
                                continue;
                            }
                            Match projectNameMatch = projectNameFinder.Match(line);
                            if (projectNameMatch.Success)
                            {
                                return owner.Create(key, projectNameMatch.Groups["name"].Value);
                            }
                        }
                    }

                    return owner.Create(key, Path.GetFileName(owner.Path));
            }
        }

        private static IEnumerable<string> FindInstallationPaths(JObject codeModel)
        {
            JObject projectTarget = codeModel;

            if (projectTarget.TryGetValue("install", out JToken installSection))
            {
                if (installSection is JObject installSectionObject)
                {
                    if (installSectionObject.TryGetValue("destinations", out var destinations))
                    {
                        if (destinations is JArray destinationsArray)
                        {
                            return destinationsArray
                                .Select(t =>
                                    (t as JObject)?.ContainsKey("path") == true ? ((JObject)t)["path"].Value<string>() : "")
                                .Where(s => !string.IsNullOrEmpty(s));
                        }
                    }
                }
            }
            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> FindExternalLibrariesInCodeModel(JObject codeModel, string projectName,
                                                                     VirtualDirectory binaryDirectory)
        {
            List<string> result = new List<string>();
            JObject projectTarget = codeModel;
            
            string cmakeSysroot = projectTarget["link"]?["sysroot"]?["path"]?.Value<string>();
            if (cmakeSysroot == null || !fileSystem.DirectoryExists(cmakeSysroot))
            {
                throw new FormattableException($"The cmake sysroot {cmakeSysroot} does not exist.");
            }
            cmakeSysroot = cmakeSysroot.CleanPath();

            JArray linkLibrariesSection = projectTarget["link"]?["commandFragments"]?.Value<JArray>();
            if (linkLibrariesSection == null)
            {
                throw new FormattableException($"The target '{projectName}' does not contain any data of type linkLibraries. " +
                                               $"The target contains the following data:{Environment.NewLine}" +
                                               $"{projectTarget.ToString(Formatting.Indented)}");
            }

            IEnumerable<string> libraries = linkLibrariesSection
                .Where(elem => elem["role"]?.Value<string>()?.Equals("libraries", StringComparison.OrdinalIgnoreCase) == true)
                .Select(elem => elem["fragment"]?.Value<string>() ?? string.Empty);

            string linkLibraries = string.Join(" ", libraries);

            List<string> rPaths = new List<string>(new[] { binaryDirectory.FullName });
            List<(int, int)> rPathMatches = new List<(int, int)>();
            Match rPathMatch = LibrariesRPathDecoder.Match(linkLibraries);
            while (rPathMatch.Success)
            {
                rPathMatches.Add((rPathMatch.Index, rPathMatch.Index + rPathMatch.Length - 1));
                rPaths.AddRange(rPathMatch.Groups["rpath"].Value.Split(';').Select(p => p.Trim('\\', '"')));
                rPathMatch = rPathMatch.NextMatch();
            }

            Match match = LibrariesDecoder.Match(linkLibraries);

            while (match.Success)
            {
                if (rPathMatches.All(((int start, int end) rMatch) =>
                                         match.Index + match.Length - 1 < rMatch.start ||
                                         match.Index > rMatch.end))
                {
                    AddExternalLib();
                }

                match = match.NextMatch();
            }

            return result;

            bool IsNotSysrootPath(string path)
            {
                path = path.CleanPath();

                return !path.StartsWith(cmakeSysroot, StringComparison.OrdinalIgnoreCase);
            }

            static bool IsNotStaticLibrary(string path)
            {
                path = path.CleanPath();

                return !StaticLibraryFileEndings.Any(e => path.EndsWith(e, StringComparison.Ordinal));
            }

            void AddExternalLib()
            {
                string path = match.Groups["path"].Value.Trim('\\', '"');
                if (path.StartsWith("-l", StringComparison.Ordinal))
                {
                    path = $"lib{path.Substring(2)}.so";
                }

                string pathBase = rPaths.FirstOrDefault(p => fileSystem.FileExists(path, p));
                if (!string.IsNullOrEmpty(pathBase) && IsNotSysrootPath(path) && IsNotStaticLibrary(path))
                {
                    VirtualFile libFile = fileSystem.GetFile(path, pathBase);
                    result.Add(libFile.FullName);
                }
            }
        }
        private JObject GetCodeModel(Entity owner, string projectName, VirtualDirectory buildSystemDirectory)
        {
            VirtualDirectory temp = FileEntity.Decorate(owner).TempDirectory;
            JObject codeModel = null;
            try
            {
                codeModel = cmakeConversation.GetCodeModel(projectName,
                    buildSystemDirectory).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (TimeoutException e)
            {
                executionContext.WriteError($"Timeout during cmake server conversation: {e}", false);
            }
            if (codeModel == null)
            {
                throw new FormattableException("Could not fetch code model from cmake build system.");
            }
            return codeModel;
        }
      
        private static string GetBuildType(Entity owner)
        {
            CommandEntity commandEntity = CommandEntity.Decorate(owner.Origin);
            string buildType = commandEntity.GetSingleValueArgument(EntityKeys.BuildTypeKey);
            if (string.IsNullOrEmpty(buildType))
            {
                buildType = Constants.ReleaseFolderName;
            }

            return buildType;
        }
    }
}
