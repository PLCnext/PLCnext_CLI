#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Deploy;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools.Events;
using System.Globalization;

namespace PlcNext.Common.Build
{
    internal partial class EngineeringLibraryBuilderExecuter
    {
        public int ExecuteSn(Entity dataModel)
        {
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);
            string projectName = string.Empty;
            try
            {
                CodeEntity projectCodeEntity = CodeEntity.Decorate(project);
                projectName = projectCodeEntity.Namespace;
            }
            catch (FormattableException e)
            {
                executionContext.WriteInformation("Namespace was not used as name because of the following error: " + e.Message, false);
                projectName = project.Name;
            }
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

            string commandOptionsFile = GenerateCommandOptionsForSharedNative(project, projectLibraries, projectName);
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

        }
        private string GenerateCommandOptionsForSharedNative(ProjectEntity project,
            Dictionary<Entity, VirtualFile> projectLibraries, string projectName)
        {
            FileEntity projectFileEntity = FileEntity.Decorate(project);
            VirtualFile commandOptions = projectFileEntity.TempDirectory.File("CommandOptions.txt");
            CommandEntity commandOrigin = CommandEntity.Decorate(project.Origin);
            VirtualDirectory outputRoot = fileSystem.GetDirectory(commandOrigin.Output, project.Path, false);
            List<string> processedFiles = new List<string>();
            executionContext.Observable.OnNext(new Change(() => { }, $"Create command options file {commandOptions.FullName}"));

            using (Stream stream = commandOptions.OpenWrite())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine($"{Constants.OutputOption} \"{MakeRelative(Path.Combine(outputRoot.FullName, projectName))}.{Constants.EngineeringLibraryExtension}\"");
                writer.WriteLine($"{Constants.GuidOption} {project.Id:D}");

                WriteLibraryFile(writer);
                WritePrecompiledLibrary(writer);
                WriteDlls(writer);
                AddAdditionalFiles(writer);
                WriteHelpFiles(writer);
                AddProperties(writer, project);
            }

            return commandOptions.FullName;

            string MakeRelative(string path)
            {
                return path.GetRelativePath(projectFileEntity.Directory.FullName);
            }

            void WriteLibraryFile(StreamWriter writer)
            {
                foreach (TargetEntity target in projectLibraries.Keys.Select(TargetEntity.Decorate))
                {
                    VirtualFile copiedLibrary = projectFileEntity
                                                .TempDirectory.Directory(target.FullName.Replace(",", "_"))
                                                .File(projectLibraries[target.Base].Name);
                    executionContext.Observable.OnNext(new Change(() => { }, $"Copy library file to {copiedLibrary.FullName}"));
                    using (Stream source = projectLibraries[target.Base].OpenRead(true))
                    using (Stream destination = copiedLibrary.OpenWrite())
                    {
                        source.CopyTo(destination);
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.EclrNativeLibraryOptionPattern,
                                                   copiedLibrary.Parent.FullName,
                                                   target.Name,
                                                   target.EngineerVersion,
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   target.ShortFullName.Replace(",", "_"),
                                                   projectName));
                }
            }

            void WritePrecompiledLibrary(StreamWriter writer) 
            {
                string fullName = Path.Combine(outputRoot.FullName, Constants.ConfigIndependentFiles, $"{projectName}.dll");
                VirtualFile precompiledLibrary = fileSystem.FileExists(fullName) 
                                                    ? fileSystem.GetFile(fullName)
                                                    : throw new LibraryNotFoundException(fullName);
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.FileOptionPattern,
                                                   Constants.PrecompiledLibraryType,
                                                   MakeRelative(precompiledLibrary.FullName),
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   "bin"));

                processedFiles.Add(fullName);
            }

            void AddAdditionalFiles(StreamWriter writer)
            {
                foreach (Entity target in projectLibraries.Keys)
                {
                    VirtualDirectory deployDirectory = DeployEntity.Decorate(target).DeployDirectory;
                    IEnumerable<VirtualFile> files = deployDirectory
                                                    .Files(searchRecursive: true).Except(projectLibraries.Values)
                                                    .Where(f => !processedFiles.Contains(f.GetRelativePath(deployDirectory)));
                    foreach (VirtualFile file in files)
                    {
                        writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                       "/file \":{0}:{1}\"",
                                                       file.GetRelativeOrAbsolutePath(projectFileEntity.Directory),
                                                       TargetEntity.Decorate(target).ShortFullName.Replace(",", "_")));
                    }
                }
            }

            void WriteDlls(StreamWriter writer)
            {
                VirtualDirectory assemblyDirectory = fileSystem.GetDirectory(Path.Combine(outputRoot.FullName, Constants.ConfigIndependentFiles));
                
                foreach (VirtualFile assembly in assemblyDirectory.Files(searchRecursive: true)
                                                                  .Where(f => !processedFiles.Contains(f.FullName)))
                {
                    string destinationPath;
                    string fileType;
                    switch (Path.GetExtension(assembly.Name)?.ToUpperInvariant() ?? string.Empty)
                    {
                        case ".DLL":
                            destinationPath = "bin";
                            fileType = string.Empty;
                            break;
                        case ".XML":
                            destinationPath = string.Empty;
                            fileType = string.Empty;
                            break;
                        default:
                            //do nothing all other files are not interesting
                            continue;
                    }
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.FileOptionPattern,
                                                   fileType,
                                                   MakeRelative(assembly.FullName),
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   destinationPath));
                }
            }

            void WriteHelpFiles(StreamWriter writer)
            {
                
                VirtualDirectory commonDirectory = fileSystem.GetDirectory(Path.Combine(outputRoot.FullName, Constants.ConfigIndependentFiles));
                if (!commonDirectory.DirectoryExists(Constants.DeployHelpDirectoryName))
                {
                    return;
                }
                VirtualDirectory helpDirectory = commonDirectory.Directory(Constants.DeployHelpDirectoryName);
                foreach (VirtualFile file in helpDirectory.Files(searchRecursive: true)
                                                                  .Where(f => !processedFiles.Contains(f.FullName)))
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                                                   Constants.FileOptionPattern,
                                                   string.Empty,
                                                   MakeRelative(file.FullName),
                                                   guidFactory.Create().ToString("D", CultureInfo.InvariantCulture),
                                                   file.Parent.FullName.GetRelativePath(commonDirectory.FullName)));
                }

            }
        }
    }
}
