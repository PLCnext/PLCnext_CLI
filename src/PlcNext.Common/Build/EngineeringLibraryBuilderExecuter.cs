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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Commands;
using PlcNext.Common.Deploy;
using PlcNext.Common.MetaData;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using CodeEntity = PlcNext.Common.CodeModel.CodeEntity;
using Entity = PlcNext.Common.DataModel.Entity;

namespace PlcNext.Common.Build
{
    internal partial class EngineeringLibraryBuilderExecuter : ILibraryBuilderExecuter
    {
        private readonly IProcessManager processManager;
        private readonly IFileSystem fileSystem;
        private readonly IGuidFactory guidFactory;
        private readonly IBinariesLocator binariesLocator;
        private readonly ExecutionContext executionContext;

        public EngineeringLibraryBuilderExecuter(IProcessManager processManager, IFileSystem fileSystem,
                                                 IGuidFactory guidFactory, IBinariesLocator binariesLocator,
                                                 ExecutionContext executionContext)
        {
            this.processManager = processManager;
            this.fileSystem = fileSystem;
            this.guidFactory = guidFactory;
            this.binariesLocator = binariesLocator;
            this.executionContext = executionContext;
        }

        //TODO Patch libmeta here
        public int Execute(Entity dataModel)
        {
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            CodeEntity projectCodeEntity = CodeEntity.Decorate(project);
            string projectName = projectCodeEntity.Namespace;
            IEnumerable<Entity> targets = project.Targets.ToArray();
            if (!targets.Any())
            {
                throw new FormattableException("Please use --target to specify for which targets the library shall be generated.");
            }
            Dictionary<Entity, VirtualFile> projectLibraries = targets.ToDictionary(t => t, FindLibrary);
            List<string> externalLibraries = new List<string>();
            List<VirtualFile> deletableFiles = new List<VirtualFile>();
            foreach (Entity target in targets)
            {
                CopyExternalLibrariesToOutputDirectory(target, projectLibraries,
                                                       out IEnumerable<string> libraries,
                                                       out IEnumerable<VirtualFile> toBeDeleted);
                externalLibraries.AddRange(libraries);
                deletableFiles.AddRange(toBeDeleted);

                ExcludeFiles(target, project, projectLibraries);
            }
            CheckMetaFiles(targets.First());

            string commandOptionsFile = GenerateCommandOptions(project, projectLibraries, projectName);
            int result = ExecuteLibraryBuilderWithCommandOptions(commandOptionsFile, project);

            foreach (VirtualFile deletable in deletableFiles)
            {
                deletable.Delete();
            }

            return result;

            VirtualFile FindLibrary(Entity target)
            {
                VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;

                string libFile = deployDirectory.Files("*.so", true)
                                    .OrderByDescending(f => f.Name.Equals($"lib{projectName}.so", StringComparison.OrdinalIgnoreCase))
                                    .ThenByDescending(f => f.LastWriteTime)
                                    .FirstOrDefault()
                                   ?.FullName;
                if (string.IsNullOrEmpty(libFile))
                {
                    throw new LibraryNotFoundException(deployDirectory.FullName);
                }

                VirtualFile file = fileSystem.GetFile(libFile);
                return file;
            }

            void CheckMetaFiles(Entity target)
            {
                TemplateEntity projectTemplateEntity = TemplateEntity.Decorate(project);
                VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;
                //VirtualDirectory configDirectory = deployDirectory.Parent.Parent.Directory("config");
                VirtualDirectory configDirectory = DeployEntity.Decorate(target).ConfigDirectory;

                if (!fileSystem.FileExists(Path.Combine(configDirectory.FullName, $"{projectName}.libmeta")))
                {
                    if (!fileSystem.FileExists(Path.Combine(deployDirectory.FullName, $"{projectName}.libmeta")))
                    {
                        throw new MetaLibraryNotFoundException(configDirectory.FullName +" or "+deployDirectory.FullName);
                    }
                }

                PatchLibmeta(fileSystem.GetFile(Path.Combine(configDirectory.FullName, $"{projectName}.libmeta")));
                IEnumerable<VirtualFile> metaFiles = configDirectory.Files(searchRecursive: true).Union(deployDirectory.Files(searchRecursive: true));

                IEnumerable<Entity> componentsWithoutMetaFile = projectTemplateEntity.EntityHierarchy
                                                                                     .Where(e => e.Type.Equals("component", StringComparison.OrdinalIgnoreCase))
                                                                                     .Where(c => !metaFiles.Any(f => f.Name.Equals($"{c.Name}.{Constants.CompmetaExtension}", 
                                                                                                                                   StringComparison.Ordinal)))
                                                                                     .ToArray();
                if (componentsWithoutMetaFile.Any())
                {
                    throw new MetaFileNotFoundException(configDirectory.FullName, $"{componentsWithoutMetaFile.First().Name}.{Constants.CompmetaExtension}");
                }

                IEnumerable<Entity> programsWithoutMetaFile = projectTemplateEntity.EntityHierarchy
                                                                                   .Where(e => e.Type.Equals("program", StringComparison.OrdinalIgnoreCase))
                                                                                   .Where(p => !metaFiles.Any(f => f.Name.Equals($"{p.Name}.{Constants.ProgmetaExtension}", 
                                                                                                                                 StringComparison.Ordinal)))
                                                                                   .ToArray();
                if (programsWithoutMetaFile.Any())
                {
                    throw new MetaFileNotFoundException(configDirectory.FullName, $"{programsWithoutMetaFile.First().Name}.{Constants.ProgmetaExtension}");
                }
            }

            void PatchLibmeta(VirtualFile libmetaFile)
            {
                if (!externalLibraries.Any())
                {
                    return;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                MetaConfigurationDocument document;
                try
                {
                    using (Stream fileStream = libmetaFile.OpenRead())
                    using (XmlReader reader = XmlReader.Create(fileStream))
                    {
                        document = (MetaConfigurationDocument)serializer.Deserialize(reader);
                    }
                }
                catch (XmlException e)
                {
                    executionContext.WriteWarning($"Cannot parse libmeta file. Cannot patch dependencies. {e.Message}");
                    return;
                }

                document.schemaVersion = "4.0";
                LibraryDefinition definition = (LibraryDefinition) document.Item;
                IEnumerable<string> dependencies = externalLibraries.Select(Path.GetFileName)
                                                                    .Concat(definition.Dependencies?.Select(d => d.path)??
                                                                            Enumerable.Empty<string>())
                                                                    .Distinct();
                definition.Dependencies = dependencies.Select(d => new DependencyDefinition {path = d}).ToArray();
                
                using (Stream fileStream = libmetaFile.OpenWrite())
                using (XmlWriter writer = XmlWriter.Create(fileStream, new XmlWriterSettings
                {
                    Indent = true
                }))
                {
                    serializer.Serialize(writer, document);
                }
            }
        }

        private static void ExcludeFiles(Entity target, ProjectEntity project, Dictionary<Entity, VirtualFile> projectLibraries)
        {
            VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;
            List<VirtualFile> filesToDelete = new List<VirtualFile>();
            foreach (string excludePattern in project.ExcludedFiles)
            {
                filesToDelete.AddRange(deployDirectory.Files(excludePattern, true).Except(projectLibraries.Values));
            }
            foreach (VirtualFile file in filesToDelete)
            {
                file.Delete();
            }
        }

        private void CopyExternalLibrariesToOutputDirectory(Entity target, Dictionary<Entity, VirtualFile> projectLibraries, 
                                                            out IEnumerable<string> externalLibraries,
                                                            out IEnumerable<VirtualFile> copiedLibraries)
        {
            VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;
            VirtualDirectory externalDirectory = deployDirectory.Directory("auto-deployed-external-libraries");
            externalDirectory.Clear();
            executionContext.Observable.OnNext(new Change(() => externalDirectory.UnClear(), "Cleared automatic copied external libraries."));
            externalLibraries = Enumerable.Empty<string>();
            copiedLibraries = Enumerable.Empty<VirtualFile>();
            List<VirtualFile> newLibraryFiles = new List<VirtualFile>();

            if (deployDirectory.Files("*.so", true).Except(projectLibraries.Values).Any())
            {
                externalLibraries = deployDirectory.Files("*.so", true).Except(projectLibraries.Values)
                                                   .Select(f => f.FullName).ToArray();
                //external libraries were copied by user; no further action is required
                return;
            }

            BuildEntity buildEntity = BuildEntity.Decorate(target);
            if (!buildEntity.HasBuildSystem)
            {
                TargetEntity targetEntity = TargetEntity.Decorate(target);
                executionContext.WriteWarning(new CMakeBuildSystemNotFoundException(targetEntity.FullName, buildEntity.BuildType).Message);
                return;
            }

            externalLibraries = buildEntity.BuildSystem.ExternalLibraries;
            TargetEntity entity = TargetEntity.Decorate(target);
            if (entity.Version >= new Version(20, 6))
            {
                foreach (string externalLibrary in buildEntity.BuildSystem.ExternalLibraries)
                {
                    VirtualFile newFile = fileSystem.GetFile(externalLibrary).CopyTo(deployDirectory);
                    newLibraryFiles.Add(newFile);
                    executionContext.Observable.OnNext(new Change(() => newFile.Delete(), $"Copied {externalLibrary} to {newFile.FullName}."));
                }
            }
            else
            {
                foreach (string externalLibrary in buildEntity.BuildSystem.ExternalLibraries)
                {
                    executionContext.WriteWarning($"The library {externalLibrary} must be transferred to the device {entity.FullName} into the directory \"usr/local/lib\" manually.");
                }
            }

            copiedLibraries = newLibraryFiles;
        }

        private string GenerateCommandOptions(ProjectEntity project, Dictionary<Entity, VirtualFile> projectLibraries, string projectName)
        {
            FileEntity projectFileEntity = FileEntity.Decorate(project);
            VirtualFile commandOptions = projectFileEntity.TempDirectory.File("CommandOptions.txt");
            CommandEntity commandOrigin = CommandEntity.Decorate(project.Origin);
            VirtualDirectory outputRoot = fileSystem.GetDirectory(commandOrigin.Output, project.Path, false);
            List<string> processedMetaFiles = new List<string>();
            executionContext.Observable.OnNext(new Change(() => { }, $"Create command options file {commandOptions.FullName}"));

            using (Stream stream = commandOptions.OpenWrite())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{Constants.OutputOption} \"{MakeRelative(Path.Combine(outputRoot.FullName, projectName))}.{Constants.EngineeringLibraryExtension}\"");
                writer.WriteLine($"{Constants.GuidOption} {project.Id:D}");

                WriteLibraryFile(writer);
                WriteMetadata(writer);
                AddAdditionalFiles(writer);
                AddProperties(writer, project);
            }

            return commandOptions.FullName;

            

            void WriteLibraryFile(StreamWriter writer)
            {
                foreach (TargetEntity target in projectLibraries.Keys.Select(TargetEntity.Decorate))
                {
                    VirtualFile copiedLibrary = projectFileEntity
                                                .TempDirectory.Directory(target.FullName.Replace(",", "_", StringComparison.Ordinal))
                                                .File(projectLibraries[target.Base].Name);
                    executionContext.Observable.OnNext(new Change(() => { }, $"Copy library file to {copiedLibrary.FullName}"));
                    using (Stream source = projectLibraries[target.Base].OpenRead(true))
                    using (Stream destination = copiedLibrary.OpenWrite())
                    {
                        source.CopyTo(destination);
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.PlcnextNativeLibraryOptionPattern,
                                                   copiedLibrary.Parent.FullName,
                                                   target.Name,
                                                   target.EngineerVersion,
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   target.ShortFullName.Replace(",", "_", StringComparison.Ordinal)));
                }
            }

            void AddAdditionalFiles(StreamWriter writer)
            {
                foreach (Entity target in projectLibraries.Keys)
                {
                    VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;
                    IEnumerable<VirtualFile> files = deployDirectory
                                                    .Files(searchRecursive: true).Except(projectLibraries.Values)
                                                    .Where(f => !processedMetaFiles.Contains(f.GetRelativePath(deployDirectory)));
                    if (files.GroupBy(f => f.Name).Any(group => group.Count() > 1))
                    {
                        throw new DuplicateFileDeployedException(files.GroupBy(f => f.Name).First(group => group.Count() > 1).Key);
                    }
                    foreach (VirtualFile file in files)
                    {
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                       "/file \":{0}:{1}\"",
                                                       file.GetRelativeOrAbsolutePath(projectFileEntity.Directory),
                                                       TargetEntity.Decorate(target).ShortFullName.Replace(",", "_", StringComparison.Ordinal)));
                    }
                }
            }

            void WriteMetadata(StreamWriter writer)
            {
                //VirtualDirectory deployDirectory = DeployEntity.Decorate(projectLibraries.Keys.First()).DeployDirectory;
                VirtualDirectory configDirectory = DeployEntity.Decorate(projectLibraries.Keys.First()).ConfigDirectory;
                HashSet<VirtualDirectory> createDirectories = new HashSet<VirtualDirectory>();
                foreach (VirtualFile metaFile in configDirectory.Files(searchRecursive: true))
                             //.Union(deployDirectory.Files(searchRecursive: true)))
                {
                    string destinationPath;
                    string fileType;
                    switch (Path.GetExtension(metaFile.Name)?.ToUpperInvariant() ?? string.Empty)
                    {
                        case ".LIBMETA":
                            destinationPath = string.Empty;
                            fileType = Constants.LibmetaFileType;
                            break;
                        case ".TYPEMETA":
                            destinationPath = string.Empty;
                            fileType = Constants.TypemetaFileType;
                            break;
                        case ".COMPMETA":
                            CreateComponentDirectory(metaFile.Parent);
                            destinationPath = metaFile.Parent.Name;
                            fileType = Constants.CompmetaFileType;
                            break;
                        case ".PROGMETA":
                            CreateProgramDirectory(metaFile.Parent);
                            destinationPath = $"{metaFile.Parent.Parent.Name}/{metaFile.Parent.Name}";
                            fileType = Constants.ProgmetaFileType;
                            break;
                        case ".DT":
                            destinationPath = string.Empty;
                            fileType = Constants.DataTypeWorksheetType;
                            break;
                        default:
                            //do nothing all other files are not interesting
                            continue;
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.FileOptionPattern,
                                                   fileType,
                                                   MakeRelative(metaFile.FullName),
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   destinationPath));
                    processedMetaFiles.Add(metaFile.GetRelativePath(configDirectory));
                }

                void CreateComponentDirectory(VirtualDirectory componentDirectory)
                {
                    if (createDirectories.Contains(componentDirectory))
                    {
                        return;
                    }

                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.DirectoryOptionPattern,
                                                   $"Logical Elements/{componentDirectory.Name}",
                                                   Constants.ComponentFolderType,
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture)));
                    createDirectories.Add(componentDirectory);
                }

                void CreateProgramDirectory(VirtualDirectory programDirectory)
                {
                    if (createDirectories.Contains(programDirectory))
                    {
                        return;
                    }

                    CreateComponentDirectory(programDirectory.Parent);
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.DirectoryOptionPattern,
                                                   $"Logical Elements/{programDirectory.Parent.Name}/{programDirectory.Name}",
                                                   Constants.ProgramFolderType,
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture)));
                    createDirectories.Add(programDirectory);
                }
            }

            string MakeRelative(string path)
            {
                return path.GetRelativePath(projectFileEntity.Directory.FullName);
            }
        }

        private static void AddProperties(StreamWriter writer, ProjectEntity project)
        {
            if (!string.IsNullOrEmpty(project.LibraryVersion))
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                                               Constants.KeyOptionPattern,
                                               Constants.LibraryVersionKey, 
                                               Escape(project.LibraryVersion)));

            }
            if (!string.IsNullOrEmpty(project.LibraryDescription))
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                               Constants.KeyOptionPattern,
                                               Constants.LibraryDescriptionKey, 
                                               Escape(project.LibraryDescription)));
            }

            if (!string.IsNullOrEmpty(project.EngineerVersion))
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                                               Constants.EngineerVersionOptionPattern,
                                               project.EngineerVersion));
            }

            if (!string.IsNullOrEmpty(project.SolutionVersion))
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                                               Constants.SolutionVersionPattern,
                                               project.SolutionVersion));
            }


            static string Escape(string value)
            {
                value = Regex.Replace(value, ",", "&#44;");
                value = Regex.Replace(value, "\"", "\\\"");
                value = Regex.Replace(value, "\n", "\\n");
                return value;
            }
        }

        private int ExecuteLibraryBuilderWithCommandOptions(string commandOptionsFile, ProjectEntity project)
        {
            FileEntity projectFileEntity = FileEntity.Decorate(project);
            string libraryBuilderName = FindLibraryBuilder();
            using (IProcess process = processManager.StartProcess(libraryBuilderName,
                                                                  $"{Constants.CommandFileOption} \"{commandOptionsFile}\"", executionContext,
                                                                  projectFileEntity.Directory.FullName))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    //Only on success delete directory.
                    projectFileEntity.TempDirectory.Delete();
                }
                return process.ExitCode;
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
        }
        
        public int ExecuteAcf(Entity dataModel)
        {
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            CodeEntity projectCodeEntity = CodeEntity.Decorate(project);
            string projectName = projectCodeEntity.Namespace;
            List<VirtualFile> processedMetaFiles = new List<VirtualFile>();
            HashSet<VirtualDirectory> createdDirectories = new HashSet<VirtualDirectory>();
            IEnumerable<Entity> targets = project.Targets.ToArray();
            if (!targets.Any())
            {
                throw new FormattableException("Please use --target to specify for which targets the library shall be generated.");
            }
            
            CheckMetaFiles();

            string commandOptionsFile = GenerateCommandOptionsForAcf();
            int result = ExecuteLibraryBuilderWithCommandOptions(commandOptionsFile, project);

            RemoveMetaFilesFromDeployDirectory();

            return result;

            void CheckMetaFiles()
            {
                TemplateEntity projectTemplateEntity = TemplateEntity.Decorate(project);
                VirtualDirectory deployDirectory = DeployEntity.Decorate(project).ConfigDirectory;

                if (!fileSystem.FileExists(Path.Combine(deployDirectory.FullName, $"{projectName}.libmeta")))
                {
                    throw new MetaLibraryNotFoundException(deployDirectory.FullName);
                }

                IEnumerable<VirtualFile> metaFiles = deployDirectory.Files(searchRecursive: true);

                IEnumerable<Entity> componentsWithoutMetaFile = projectTemplateEntity.EntityHierarchy
                                                                                     .Where(e => e.Type.Equals("acfcomponent", StringComparison.OrdinalIgnoreCase))
                                                                                     .Where(c => !metaFiles.Any(f => f.Name.Equals($"{c.Name}.{Constants.CompmetaExtension}",
                                                                                                                                   StringComparison.Ordinal)))
                                                                                     .ToArray();
                if (componentsWithoutMetaFile.Any())
                {
                    throw new MetaFileNotFoundException(deployDirectory.FullName, $"{componentsWithoutMetaFile.First().Name}.{Constants.CompmetaExtension}");
                }
            }
            
            string GenerateCommandOptionsForAcf()
            {
                FileEntity projectFileEntity = FileEntity.Decorate(project);
                VirtualFile commandOptions = projectFileEntity.TempDirectory.File("CommandOptions.txt");
                CommandEntity commandOrigin = CommandEntity.Decorate(project.Origin);
                VirtualDirectory outputRoot = fileSystem.GetDirectory(commandOrigin.Output, project.Path, false);
                executionContext.Observable.OnNext(new Change(() => { }, $"Create command options file {commandOptions.FullName}"));

                using (Stream stream = commandOptions.OpenWrite())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine($"{Constants.OutputOption} \"{MakeRelative(Path.Combine(outputRoot.FullName, projectName))}.{Constants.EngineeringLibraryExtension}\"");
                    writer.WriteLine($"{Constants.GuidOption} {project.Id:D}");

                    WriteMetadata(writer);

                    AddProperties(writer, project);
                }

                return commandOptions.FullName;


                void WriteMetadata(StreamWriter writer)
                {
                    VirtualDirectory deployDirectory = DeployEntity.Decorate(targets.First()).DeployDirectory;
                    VirtualDirectory configDirectory = DeployEntity.Decorate(project).ConfigDirectory;
                    foreach (VirtualFile metaFile in configDirectory.Files(searchRecursive: true)
                                                                    .Union(deployDirectory.Files(searchRecursive: true)))
                    {
                        string destinationPath;
                        string fileType;
                        switch (Path.GetExtension(metaFile.Name)?.ToUpperInvariant() ?? string.Empty)
                        {
                            case ".LIBMETA":
                                destinationPath = string.Empty;
                                fileType = Constants.LibmetaFileType;
                                break;
                            case ".TYPEMETA":
                                destinationPath = string.Empty;
                                fileType = Constants.TypemetaFileType;
                                break;
                            case ".COMPMETA":
                                CreateComponentDirectory(metaFile.Parent);
                                destinationPath = metaFile.Parent.Name;
                                fileType = Constants.CompmetaFileType;
                                break;
                            case ".CONFIG":
                                if (!metaFile.Name.EndsWith(".acf.config", StringComparison.OrdinalIgnoreCase))
                                    continue;
                                destinationPath = string.Empty;
                                fileType = Constants.AcfConfigurationType;
                                break;
                            case ".DT":
                                destinationPath = string.Empty;
                                fileType = Constants.DataTypeWorksheetType;
                                break;
                            default:
                                //do nothing all other files are not interesting
                                continue;
                        }
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                       Constants.FileOptionPattern,
                                                       fileType,
                                                       MakeRelative(metaFile.FullName),
                                                       guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                       destinationPath));
                        processedMetaFiles.Add(metaFile);
                    }

                    void CreateComponentDirectory(VirtualDirectory componentDirectory)
                    {
                        if (createdDirectories.Contains(componentDirectory))
                        {
                            return;
                        }

                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                       Constants.DirectoryOptionPattern,
                                                       $"Logical Elements/{componentDirectory.Name}",
                                                       Constants.ComponentFolderType,
                                                       guidFactory.Create().ToString("D", CultureInfo.InvariantCulture)));
                        createdDirectories.Add(componentDirectory);
                    }
                }

                string MakeRelative(string path)
                {
                    return path.GetRelativePath(projectFileEntity.Directory.FullName);
                }
            }
        
            void RemoveMetaFilesFromDeployDirectory()
            {
                VirtualDirectory deployDirectory = DeployEntity.Decorate(targets.First()).DeployDirectory;

                
                foreach (VirtualFile metaFile in 
                    processedMetaFiles.Where(file => !file.Name.EndsWith(".acf.config", StringComparison.OrdinalIgnoreCase)))
                {
                    metaFile.Delete();
                }

                foreach (VirtualDirectory directory in createdDirectories.Where(d => !d.Entries.Any()))
                {
                    directory.Delete();
                }
            }
        }
    }
}
