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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
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
        public DeployService(IFileSystem fileSystem, ITemplateResolver resolver, ITargetParser targetParser)
        {
            this.fileSystem = fileSystem;
            this.resolver = resolver;
            this.targetParser = targetParser;
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
                throw new FormattableException
                    ("Please use --target to specify for which targets the deploy shall be executed.");
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

                if (!fileSystem.FileExists(from, project.Path))
                    throw new FormattableException($"The source file {from} does not exist.");

                if (!string.IsNullOrEmpty(rawTarget))
                {
                    DeployFileForRawTarget(rawTarget);
                }
                else
                {
                    foreach (Target target in targets)
                    {
                        DeployFileForTarget(target);
                    }
                }

                void DeployFileForRawTarget(string target)
                {
                    Target parsedTarget = targetParser.ParseTarget(target, null, targets);
                    DeployFileForTarget(parsedTarget);
                }

                void DeployFileForTarget(Target target)
                {
                    VirtualFile fileToCopy = fileSystem.GetFile(from, project.Path);
                    VirtualFile copiedFile = fileSystem.GetDirectory(to, GetOutputDirectory(target).FullName).File(fileToCopy.Name);

                    using (Stream source = fileToCopy.OpenRead(true))
                    using (Stream destination = copiedFile.OpenWrite())
                    {
                        destination.SetLength(0);
                        source.CopyTo(destination);
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
                    if (file.deployPath == null)
                        continue;

                    VirtualFile deployableFile = GetFile(deployableEntity, file, dataModel.Root.Path);

                    foreach (Target target in targets)
                    {
                        VirtualFile destination = GetDestination(file, target, deployableFile.Name);

                        using (Stream source = deployableFile.OpenRead(true))
                        using (Stream dest = destination.OpenWrite())
                        {
                            dest.SetLength(0);
                            source.CopyTo(dest);
                        }
                    }
                }
                VirtualFile GetDestination(templateFile file, Target target, string name)
                {
                    string basePath = GetOutputDirectory(target).FullName;
                    string path = resolver.Resolve(file.deployPath ?? string.Empty, dataModel);

                    VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), basePath);
                    return destination;

                }
            }

            VirtualDirectory GetOutputDirectory(Target target)
            {

                string basePath = project.Path;
                if (!command.IsCommandArgumentSpecified(Constants.OutputArgumentName))
                {

                    return fileSystem.GetDirectory(Path.Combine(basePath, Constants.LibraryFolderName, target.GetFullName().Replace(',', '_'),
                        Constants.ReleaseFolderName));
                }

                basePath = fileSystem.GetDirectory(command.GetSingleValueArgument(Constants.OutputArgumentName), basePath).FullName;
                if (targets.Count() > 1)
                    basePath = Path.Combine(basePath, target.GetFullName().Replace(',', '_'), Constants.ReleaseFolderName);
                    
                return fileSystem.GetDirectory(basePath);
            }
        }

        private VirtualFile GetFile(Entity dataModel, templateFile file, string basePath)
        {
            string path = resolver.Resolve(file.path ?? string.Empty, dataModel);
            string name = resolver.Resolve(file.name, dataModel);

            VirtualFile destination = fileSystem.GetFile(Path.Combine(path, name), basePath);
            return destination;
        }
    }
}
