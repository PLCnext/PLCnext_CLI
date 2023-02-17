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
using System.Text.RegularExpressions;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Deploy
{
    internal class DeployService : IDeployService
    {
        private static readonly Regex FilesDecoder = new Regex("^(?<from>[^\\|]+)\\|(?<destination>[^\\|]+)(\\|(?<target>.+))?$", RegexOptions.Compiled);
        private readonly IFileSystem fileSystem;
        private readonly ITemplateResolver resolver;
        private readonly ITargetParser targetParser;
        private readonly ExecutionContext executionContext;
        public DeployService(IFileSystem fileSystem, ITemplateResolver resolver, ITargetParser targetParser, ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.resolver = resolver;
            this.targetParser = targetParser;
            this.executionContext = executionContext;
        }

        public void DeployFiles(Entity dataModel)
        {
            IEnumerable<Entity> deployableEntities = dataModel.Root.Hierarchy();
            ProjectEntity project = ProjectEntity.Decorate(dataModel.Root);

            // get targets to deploy for
            IEnumerable<Target> targets = null;

            CommandEntity command = CommandEntity.Decorate(dataModel);
            
            if (command.IsCommandArgumentSpecified(Constants.TargetArgumentName))
            {
                
                IEnumerable<string> rawTargets = command.GetMultiValueArgument(Constants.TargetArgumentName);

                if (rawTargets.Any())
                {
                    targets = targetParser.GetSpecificTargets(rawTargets, false).Select(t => t.Item1);
                }
                else
                {
                    targets = targetParser.Targets(project, false).ValidTargets;
                }
            }
            else
            {
                targets = targetParser.Targets(project, false).ValidTargets;
            }
            if (!targets.Any())
            {
                throw new NoTargetSpecifiedException();
            }


            foreach (Entity deployableEntity in deployableEntities)
            {
                DeployFilesFromTemplate(deployableEntity);
            }

            if (command.IsCommandArgumentSpecified(Constants.FilesArgumentName))
            {
                IEnumerable<string> files = command.GetMultiValueArgument(Constants.FilesArgumentName);
                
                foreach (string file in files)
                {
                    DeployFileFromArgument(file);
                }
            }

            void DeployFileFromArgument(string file)
            {
                Match match = FilesDecoder.Match(file);
                if (!match.Success)
                    throw new FormattableException($"The input {file} could not be parsed. Expected pattern is <fileLocation>|<destination>[|<target>]");

                string from = match.Groups["from"].Value;
                string to = match.Groups["destination"].Value;
                string rawTarget = match.Groups["target"].Value;

                VirtualDirectory baseDirectory = null;
                string relativePath = null;
                bool recreateStructure = false;
                string[] path = fileSystem.GetPath(from);
                int firstWildCard = path.TakeWhile(p => !p.Contains('*') && !p.Contains('?')).Count();
                if (firstWildCard != path.Length)
                {
                    baseDirectory = fileSystem.GetDirectory(Path.Combine(path.Take(firstWildCard).ToArray()), project.Path, false);
                    relativePath = Path.Combine(path.Skip(firstWildCard).ToArray());
                    recreateStructure = true;
                }

                if (recreateStructure)
                {
                    IEnumerable<VirtualFile> deployFiles = baseDirectory.Files(relativePath, true).ToArray();
                    if (!deployFiles.Any())
                    {
                        throw new DeployFileNotFoundException(@from);
                    }
                    foreach (VirtualFile deployFile in deployFiles)
                    {
                        string structure = Path.GetDirectoryName(deployFile.GetRelativePath(baseDirectory));
                        string fileDestination = string.IsNullOrEmpty(structure) ? to : Path.Combine(to, structure);
                        DeployFile(deployFile.FullName, fileDestination);
                    }
                }
                else if (fileSystem.FileExists(from, project.Path))
                {
                    DeployFile(from, to);
                }
                else
                {
                    throw new DeployFileNotFoundException(@from);
                }

                void DeployFile(string sourceFile, string destinationDirectory)
                {
                    if (!string.IsNullOrEmpty(rawTarget))
                    {
                        DeployFileForRawTarget(rawTarget, sourceFile, destinationDirectory);
                    }
                    else
                    {
                        foreach (Target target in targets)
                        {
                            DeployFileForTarget(target, sourceFile, destinationDirectory);
                        }
                    }
                }

                void DeployFileForRawTarget(string target, string sourceFile, string destinationDirectory)
                {
                    Target parsedTarget = targetParser.ParseTarget(target, null, targets);
                    DeployFileForTarget(parsedTarget, sourceFile, destinationDirectory);
                }

                void DeployFileForTarget(Target target, string sourceFile, string destinationDirectory)
                {
                    VirtualFile fileToCopy = fileSystem.GetFile(sourceFile, project.Path);
                    VirtualFile copiedFile = fileSystem.GetDirectory(destinationDirectory, GetOutputDirectory(target).FullName).File(fileToCopy.Name);

                    using (Stream source = fileToCopy.OpenRead(true))
                    using (Stream destination = copiedFile.OpenWrite())
                    {
                        destination.SetLength(0);
                        source.CopyTo(destination);

                        executionContext.WriteVerbose($"Deployed file {fileToCopy.FullName} to {copiedFile.FullName}.");
                    }
                }
            }

            void DeployFilesFromTemplate(Entity deployableEntity)
            {
                TemplateDescription template = deployableEntity.Template();
                if (template == null)
                {
                    return;
                }

                foreach (templateFile file in template.File)
                {
                    if (!file.deployPathSpecified)
                        continue;

                    VirtualFile deployableFile = GetFile(file, dataModel.Root.Path, false, out string path);

                    DeployFile(file, deployableFile, path);
                }

                foreach (templateGeneratedFile generatedFile in template.GeneratedFile??Enumerable.Empty<templateGeneratedFile>())
                {
                    if (!generatedFile.deployPathSpecified)
                        continue;

                    VirtualFile deployableFile = GetFile(generatedFile, dataModel.Root.Path, true, out string path);

                    DeployFile(generatedFile, deployableFile, path);
                }

                VirtualFile GetDestination(templateFile file, Target target, string name)
                {
                    string basePath = GetOutputDirectory(target).FullName;
                    string path = resolver.Resolve(file.deployPath ?? string.Empty, deployableEntity);

                    VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), basePath);
                    return destination;
                }

                void DeployFile(templateFile file, VirtualFile deployableFile, string filePath)
                {
                    if (deployableFile == null)
                    {
                        executionContext.WriteVerbose($"Could not find file {filePath} in {dataModel.Root.Path}, the file will not be deployed.");
                        return;
                    }
                    foreach (Target target in targets)
                    {
                        VirtualFile destination = GetDestination(file, target, deployableFile.Name);

                        using (Stream source = deployableFile.OpenRead(true))
                        using (Stream dest = destination.OpenWrite())
                        {
                            dest.SetLength(0);
                            source.CopyTo(dest);
                        }

                        executionContext.WriteVerbose($"Deployed file {deployableFile.FullName} to {destination.FullName}.");
                    }
                }

                VirtualFile GetFile(templateFile file, string basePath, bool isGeneratedFile, out string realFilePath)
                {
                    string path = resolver.Resolve(file.path ?? string.Empty, deployableEntity);
                    string name = resolver.Resolve(file.name, deployableEntity);

                    if (isGeneratedFile && file is templateGeneratedFile generatedFile)
                    {
                        realFilePath = Path.Combine(path, name);
                        if (!Path.IsPathRooted(realFilePath))
                        {
                            realFilePath = Path.Combine(Constants.IntermediateFolderName, generatedFile.generator?.ToLowerInvariant() ?? string.Empty, realFilePath);
                        }
                    }
                    else
                    {
                        realFilePath = Path.Combine(path, name);
                    }

                    VirtualFile destination = fileSystem.FileExists(realFilePath, basePath)
                                                  ? fileSystem.GetFile(realFilePath, basePath)
                                                  : null;
                    return destination;
                }
            }

            VirtualDirectory GetOutputDirectory(Target target)
            {
                string buildTypeFolder = command.IsCommandArgumentSpecified(Constants.BuildTypeArgumentName)
                                             ? FormatBuildType(command.GetSingleValueArgument(Constants.BuildTypeArgumentName))
                                             : Constants.ReleaseFolderName;
                string basePath = project.Path;
                if (!command.IsCommandArgumentSpecified(Constants.OutputArgumentName))
                {

                    return fileSystem.GetDirectory(Path.Combine(basePath, Constants.LibraryFolderName, target.GetFullName().Replace(',', '_'),
                                                                buildTypeFolder));
                }

                basePath = fileSystem.GetDirectory(command.GetSingleValueArgument(Constants.OutputArgumentName), basePath).FullName;
                basePath = Path.Combine(basePath, target.GetFullName().Replace(',', '_'), buildTypeFolder);

                return fileSystem.GetDirectory(basePath);

                string FormatBuildType(string buildType)
                {
                    if (string.IsNullOrEmpty(buildType))
                    {
                        return Constants.ReleaseFolderName;
                    }

                    return buildType.Substring(0, 1).ToUpperInvariant() + 
                           buildType.Substring(1).ToLowerInvariant();
                }
            }
        }
    }
}
