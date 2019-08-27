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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using CodeEntity = PlcNext.Common.CodeModel.CodeEntity;
using Entity = PlcNext.Common.DataModel.Entity;

namespace PlcNext.Common.Build
{
    internal class EngineeringLibraryBuilderExecuter : ILibraryBuilderExecuter
    {
        private readonly IProcessManager processManager;
        private readonly IFileSystem fileSystem;
        private readonly IGuidFactory guidFactory;
        private readonly IBinariesLocator binariesLocator;
        private readonly ICMakeConversation cmakeConversation;

        private static readonly Regex LibrariesDecoder = new Regex("(?<path>\\\"[^\\\"]+\\\"|[^ |\\\"]+)", RegexOptions.Compiled);

        public EngineeringLibraryBuilderExecuter(IProcessManager processManager, IFileSystem fileSystem,
                                                 IGuidFactory guidFactory, IBinariesLocator binariesLocator,
                                                 ICMakeConversation cmakeConversation)
        {
            this.processManager = processManager;
            this.fileSystem = fileSystem;
            this.guidFactory = guidFactory;
            this.binariesLocator = binariesLocator;
            this.cmakeConversation = cmakeConversation;
        }

        public int Execute(ProjectEntity project, string metaFilesDirectory, string libraryLocation,
                           string outputDirectory,
                           ChangeObservable observable, IUserInterface userInterface, Guid libraryGuid,
                           IEnumerable<(Target, string)> targets,
                           Dictionary<Target, IEnumerable<VirtualFile>> externalLibraries)
        {
            FileEntity projectFileEntity = FileEntity.Decorate(project);
            CodeEntity projectCodeEntity = CodeEntity.Decorate(project);
            TemplateEntity projectTemplateEntity = TemplateEntity.Decorate(project);
            string projectName = projectCodeEntity.Namespace;

            VirtualDirectory tempDirectory;
            Dictionary<Target, string> targetToBuildTypeDictionary = new Dictionary<Target, string>();

            UpdateLocations();
            HashSet<(VirtualFile, Target)> libraries = FindLibraries();
            string libraryBuilderName = FindLibraryBuilder();
            HashSet<(VirtualFile, Target)> resolvedExternalLibraries = FindExternalLibraries();
            string commandOptionsFile = GenerateCommandOptionsFile();
            using (IProcess process = processManager.StartProcess(libraryBuilderName,
                                                                  $"{Constants.CommandFileOption} \"{commandOptionsFile}\"", userInterface,
                                                                  projectFileEntity.Directory.FullName))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    //Only on success delete directory.
                    tempDirectory.Delete();
                }
                return process.ExitCode;
            }

            void UpdateLocations()
            {
                tempDirectory = fileSystem.GetTemporaryDirectory();
                observable.OnNext(new Change(() => { }, $"Create temporary folder {tempDirectory.FullName}"));

                if (string.IsNullOrEmpty(metaFilesDirectory))
                {
                    metaFilesDirectory = projectFileEntity.Directory
                                                .Directory(Constants.IntermediateFolderName)
                                                .Directory(Constants.MetadataFolderName)
                                                .FullName;
                }

                if (!fileSystem.FileExists(Path.Combine(metaFilesDirectory, $"{projectName}.libmeta")))
                {
                    throw new MetaLibraryNotFoundException(metaFilesDirectory);
                }

                CheckMetaFilesExist();
                
                if (string.IsNullOrEmpty(outputDirectory))
                {
                    outputDirectory = projectFileEntity.Directory
                                             .Directory(Constants.LibraryFolderName)
                                             .FullName;
                }

                if (string.IsNullOrEmpty(libraryLocation))
                {
                    libraryLocation = projectFileEntity.Directory
                                             .Directory(Constants.LibraryFolderName)
                                             .FullName;
                }

                void CheckMetaFilesExist()
                {
                    IEnumerable<VirtualFile> metaFiles = fileSystem.GetDirectory(metaFilesDirectory).Files(searchRecursive: true);

                    IEnumerable<Entity> componentsWithoutMetaFile = projectTemplateEntity.EntityHierarchy
                        .Where(e => e.Type.Equals("component", StringComparison.OrdinalIgnoreCase))
                        .Where(c => !metaFiles.Any(f => f.Name.Equals($"{c.Name}.{Constants.CompmetaExtension}")));
                    if (componentsWithoutMetaFile.Any())
                    {
                        throw new MetaFileNotFoundException(metaFilesDirectory, $"{componentsWithoutMetaFile.First().Name}.{Constants.CompmetaExtension}");
                    }

                    IEnumerable<Entity> programsWithoutMetaFile = projectTemplateEntity.EntityHierarchy
                        .Where(e => e.Type.Equals("program", StringComparison.OrdinalIgnoreCase))
                        .Where(p => !metaFiles.Any(f => f.Name.Equals($"{p.Name}.{Constants.ProgmetaExtension}")));
                    if (programsWithoutMetaFile.Any())
                    {
                        throw new MetaFileNotFoundException(metaFilesDirectory, $"{programsWithoutMetaFile.First().Name}.{Constants.ProgmetaExtension}");
                    }
                }
            }

            HashSet<(VirtualFile, Target)> FindLibraries()
            {
                HashSet<(VirtualFile, Target)> libs = new HashSet<(VirtualFile, Target)>();

                targets = targets
                    .GroupBy(t => t.Item1.GetShortFullName())
                    .Select(g => g.Where(x => x.Item1.Version == g.Select(z => z.Item1.Version).Max()).FirstOrDefault());

                foreach ((Target, string) target in targets)
                {
                    string libFile = target.Item2;
                    if (string.IsNullOrEmpty(libFile))
                    {
                        libFile = fileSystem.GetDirectory(libraryLocation)
                                                    .Directory(target.Item1.GetFullName().Replace(",", "_"))
                                                    .Files("*.so", true)
                                                    .OrderByDescending(f => f.Name.Equals($"lib{projectName}.so"))
                                                    .ThenByDescending(f => f.FullName.Contains(Constants.ReleaseFolderName))
                                                    .ThenByDescending(f => f.LastWriteTime)
                                                    .FirstOrDefault()
                                                    ?.FullName;
                        if (string.IsNullOrEmpty(libFile))
                        {
                            throw new LibraryNotFoundException(fileSystem.GetDirectory(libraryLocation)
                                                        .Directory(target.Item1.GetFullName().Replace(",", "_")).FullName);
                        }
                    }
                    else
                    {
                        if (!fileSystem.FileExists(libFile))
                        {
                            if (fileSystem.DirectoryExists(libFile))
                            {
                                libFile = fileSystem.GetDirectory(libFile)
                                                    .Files("*.so", true)
                                                    .OrderByDescending(f => f.Name.Equals($"lib{projectName}.so"))
                                                    .ThenByDescending(f => f.FullName.Contains(Constants.ReleaseFolderName))
                                                    .ThenByDescending(f => f.LastWriteTime)
                                                    .FirstOrDefault()
                                                    ?.FullName;
                            }
                        }
                    }

                    if (!fileSystem.FileExists(libFile))
                    {
                        throw new LibraryNotFoundException(libFile);
                    }

                    string buildType = Constants.ReleaseFolderName;
                    if (libFile.Contains(Constants.DebugFolderName))
                    {
                        buildType = Constants.DebugFolderName;
                    }
                    targetToBuildTypeDictionary.Add(target.Item1, buildType);


                    VirtualFile renamedLibrary = tempDirectory.Directory(target.Item1.GetFullName().Replace(",", "_")).File("lib" + projectName + Path.GetExtension(libFile));
                    observable.OnNext(new Change(() => { }, $"Rename library file to {renamedLibrary.FullName}"));
                    using (Stream source = fileSystem.GetFile(libFile).OpenRead(true))
                    using (Stream destination = renamedLibrary.OpenWrite())
                    {
                        source.CopyTo(destination);
                    }
                    libs.Add((renamedLibrary, target.Item1));
                }
                return libs;

            }

            HashSet<(VirtualFile, Target)> FindExternalLibraries()
            {
                HashSet<(VirtualFile, Target)> externalLibs = new HashSet<(VirtualFile, Target)>();

                if (externalLibraries == null || externalLibraries.Count() == 0)
                {
                    try
                    {
                        foreach ((Target, string) target in targets)
                        {
                            GetLibsFromCMakeServer(target.Item1);
                        }
                    }
                    catch (CMakeBuildSystemNotFoundException e)
                    {
                        userInterface.WriteWarning(e.Message);
                        externalLibs.Clear();
                    }
                    return externalLibs;
                }

                foreach(KeyValuePair<Target, IEnumerable<VirtualFile>> kvp in externalLibraries)
                {
                    foreach(VirtualFile file in kvp.Value)
                    {
                        externalLibs.Add((file, kvp.Key));
                    }
                }
                return externalLibs;


                void GetLibsFromCMakeServer(Target target){

                    string buildType = Constants.ReleaseFolderName;
                    if (targetToBuildTypeDictionary.ContainsKey(target))
                    {
                        buildType = targetToBuildTypeDictionary[target];
                    }

                    string binaryDirectory = Path.Combine(projectFileEntity.Directory.FullName, Constants.IntermediateFolderName,
                                                                                     Constants.CmakeFolderName,
                                                                                     target.GetFullName(),
                                                                                     buildType);
                    if (!fileSystem.DirectoryExists(binaryDirectory))
                    {
                        throw new CMakeBuildSystemNotFoundException(binaryDirectory);
                    }

                    JArray codeModel = cmakeConversation.GetCodeModelFromServer(tempDirectory.Directory("cmake"),
                                                                                projectFileEntity.Directory,
                                                                                fileSystem.GetDirectory(binaryDirectory));
                    ExploreCMakeOutput();

                    void ExploreCMakeOutput()
                    {
                        if(codeModel == null)
                        {
                            throw new FormattableException("Could not fetch codemodel from cmake build system.");
                        }

                        JObject projectTarget = codeModel.GetProjectTarget(project.Name);

                        string cmakeSysroot = projectTarget["sysroot"].Value<string>();
                        if (cmakeSysroot == null || !fileSystem.DirectoryExists(cmakeSysroot))
                        {
                            throw new FormattableException($"The cmake sysroot {cmakeSysroot} does not exist.");
                        }
                        cmakeSysroot = cmakeSysroot.CleanPath();

                        string linkLibraries = projectTarget["linkLibraries"].Value<string>();
                        if (linkLibraries == null)
                        {
                            throw new FormattableException($"The target '{project.Name}' does not contain any data of type linkLibraries. " +
                                                           $"The target contains the following data:{Environment.NewLine}" +
                                                           $"{projectTarget.ToString(Formatting.Indented)}");
                        }

                        Match match = LibrariesDecoder.Match(linkLibraries);

                        while (match.Success)
                        {
                            string path = match.Groups["path"].Value;

                            if (fileSystem.FileExists(path, binaryDirectory) && IsNotSysrootPath(path)) { 

                               VirtualFile libFile = fileSystem.GetFile(path, binaryDirectory);
                                externalLibs.Add((libFile, target));
                            }
                            match = match.NextMatch();
                        }

                        bool IsNotSysrootPath(string path)
                        {
                            path = path.CleanPath();
                            
                            return !path.StartsWith(cmakeSysroot, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
            }

            string FindLibraryBuilder()
            {
                string command = binariesLocator.GetExecutableCommand("EngineeringLibraryBuilder");
                if (string.IsNullOrEmpty(command))
                {
                    throw new LibraryBuilderNotFoundException();
                }

                return command;
            }

            string GenerateCommandOptionsFile()
            {
                VirtualFile commandOptions = tempDirectory.File("CommandOptions.txt");
                observable.OnNext(new Change(() => { }, $"Create command options file {commandOptions.FullName}"));


                using (Stream stream = commandOptions.OpenWrite())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"{Constants.OutputOption} \"{MakeRelative(Path.Combine(outputDirectory, projectName))}.{Constants.EngineeringLibraryExtension}\"");
                    writer.WriteLine($"{Constants.GuidOption} {GetLibraryId():D}");
                    foreach ((VirtualFile libraryFile, Target target) in libraries)
                    {
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.PlcnextNativeLibraryOptionPattern,
                                                   libraryFile.Parent.FullName,
                                                   target.Name,
                                                   DeviceVersion(target),
                                                   guidFactory.Create().ToString("D"),
                                                   target.GetShortFullName().Replace(",", "_")));
                    }

                    foreach ((VirtualFile externalLibFile, Target target) in resolvedExternalLibraries)
                    {
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                    "/file \":{0}:{1}\"",
                                                    externalLibFile.FullName,
                                                    target.GetShortFullName().Replace(",", "_")));
                    }

                    VirtualDirectory metaDirectory = fileSystem.GetDirectory(metaFilesDirectory);
                    HashSet<VirtualDirectory> createDirectories = new HashSet<VirtualDirectory>();
                    foreach (VirtualFile metaFile in metaDirectory.Files(searchRecursive: true))
                    {
                        string destinationPath;
                        string fileType;
                        switch (Path.GetExtension(metaFile.Name)?.ToLowerInvariant() ?? string.Empty)
                        {
                            case ".libmeta":
                                destinationPath = string.Empty;
                                fileType = Constants.LibmetaFileType;
                                break;
                            case ".typemeta":
                                destinationPath = string.Empty;
                                fileType = Constants.TypemetaFileType;
                                break;
                            case ".compmeta":
                                CreateComponentDirectory(metaFile.Parent, createDirectories, writer);
                                destinationPath = metaFile.Parent.Name;
                                fileType = Constants.CompmetaFileType;
                                break;
                            case ".progmeta":
                                CreateProgramDirectory(metaFile.Parent, createDirectories, writer);
                                destinationPath = $"{metaFile.Parent.Parent.Name}/{metaFile.Parent.Name}";
                                fileType = Constants.ProgmetaFileType;
                                break;
                            default:
                                //do nothing all other files are not interesting
                                continue;
                        }
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                       Constants.FileOptionPattern,
                                                       fileType,
                                                       MakeRelative(metaFile.FullName),
                                                       guidFactory.Create().ToString("D"),
                                                       destinationPath));
                    }
                    
                    string DeviceVersion(Target target)
                    {
                        string possibleVersion = target.LongVersion.Trim().Split(' ')[0];
                        if (Version.TryParse(possibleVersion, out Version version))
                        {
                            int parts = possibleVersion.Split('.').Length;
                            if (parts == 2)
                            {
                                return $"{version.ToString(2)}.0";
                            }

                            if (parts > 2)
                            {
                                return version.ToString(3);
                            }
                        }

                        return target.ShortVersion;
                    }
                }

                return commandOptions.FullName;

                Guid GetLibraryId()
                {
                    if (libraryGuid != default(Guid))
                    {
                        return libraryGuid;
                    }

                    if (!project.Settings.IsPersistent)
                    {
                        userInterface.WriteWarning("The id for the library will change for each generation please use the --id option to set the id.");
                        return guidFactory.Create();
                    }

                    string storedId = project.Settings.Value.Id;
                    if (string.IsNullOrEmpty(storedId))
                    {
                        storedId = guidFactory.Create().ToString("D");
                        project.Settings.SetId(storedId);
                    }
                    return Guid.Parse(storedId);
                }

                void CreateComponentDirectory(VirtualDirectory componentDirectory,
                                              ISet<VirtualDirectory> createDirectories, StreamWriter writer)
                {
                    if (createDirectories.Contains(componentDirectory))
                    {
                        return;
                    }

                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.DirectoryOptionPattern,
                                                   $"Logical Elements/{componentDirectory.Name}",
                                                   Constants.ComponentFolderType,
                                                   guidFactory.Create().ToString("D")));
                    createDirectories.Add(componentDirectory);
                }

                void CreateProgramDirectory(VirtualDirectory programDirectory, ISet<VirtualDirectory> createDirectories,
                                            StreamWriter writer)
                {
                    if (createDirectories.Contains(programDirectory))
                    {
                        return;
                    }

                    CreateComponentDirectory(programDirectory.Parent, createDirectories, writer);
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.DirectoryOptionPattern,
                                                   $"Logical Elements/{programDirectory.Parent.Name}/{programDirectory.Name}",
                                                   Constants.ProgramFolderType,
                                                   guidFactory.Create().ToString("D")));
                    createDirectories.Add(programDirectory);
                }

                string MakeRelative(string path)
                {
                    return path.GetRelativePath(projectFileEntity.Directory.FullName);
                }
            }
        }
    }
}
