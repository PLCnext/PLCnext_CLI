#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace PlcNext.Common.Project
{
    internal class ProjectConfigurationProvider : PriorityContentProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ExecutionContext executionContext;

        public override SubjectIdentifier HigherPrioritySubject { get; } = new SubjectIdentifier(nameof(CommandDefinitionContentProvider));

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);


        public ProjectConfigurationProvider(IFileSystem fileSystem, ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.executionContext = executionContext;
        }
        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.ProjectConfigurationsKey ||
                   key == EntityKeys.EngineerVersionKey ||
                   key == EntityKeys.SolutionVersionKey ||
                   key == EntityKeys.LibraryDescriptionKey ||
                   key == EntityKeys.LibraryVersionKey ||
                   key == EntityKeys.ExcludeFilesKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.ProjectConfigurationsKey:
                    return GetProjectConfiguration();
                case EntityKeys.EngineerVersionKey:
                    return GetEngineerVersion();
                case EntityKeys.SolutionVersionKey:
                    return GetSolutionVersion();
                case EntityKeys.LibraryDescriptionKey:
                    return GetLibraryDescription();
                case EntityKeys.LibraryVersionKey:
                    return GetLibraryVersion();
                case EntityKeys.ExcludeFilesKey:
                    return GetExcludedFiles();
                default:
                    throw new ContentProviderException(key, owner);
            }

            Entity GetProjectConfiguration()
            {
                
                if (!owner.HasPath)
                {
                    executionContext.WriteVerbose("The executed command lacks the path argument, no project configuration can be loaded.");
                    return owner.Create(key, new ProjectConfigurations());
                }

                string rootFilePath = fileSystem.GetDirectory(owner.Path, createNew: false)
                                                .FullName;

                string projectDirectory = fileSystem.FileExists(rootFilePath)
                                            ? Path.GetDirectoryName(rootFilePath)
                                            : rootFilePath;

                VirtualFile file = fileSystem.DirectoryExists(projectDirectory) 
                                    && fileSystem.FileExists(Path.Combine(projectDirectory, Constants.ConfigFileName))
                                           ? fileSystem.GetFile(Path.Combine(projectDirectory, Constants.ConfigFileName))
                                           : null;
                if (file == null)
                {
                    executionContext.WriteVerbose($"No config file found in {projectDirectory}.");
                    return owner.Create(key, new ProjectConfigurations());
                }

                try
                {
                    using (Stream fileStream = file.OpenRead())
                    using (XmlReader reader = XmlReader.Create(fileStream))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ProjectConfiguration));
                        ProjectConfiguration configuration = (ProjectConfiguration)serializer.Deserialize(reader);
                        if (configuration != null)
                        {
                            return owner.Create(key, new ProjectConfigurations(configuration, file));
                        }
                    }
                }
                catch (DeployArgumentsException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    executionContext.WriteVerbose($"Error while trying to parse project configuration {file.FullName}." +
                                                          $"{Environment.NewLine}{e}");
                }

                return owner.Create(key, new ProjectConfigurations());

            }

            Entity GetEngineerVersion()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (command.CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)
                    && command.IsCommandArgumentSpecified(Constants.EngineerVersionArgumentKey))
                {
                    if (command.IsCommandArgumentSpecified(Constants.SolutionVersionArgumentKey))
                    {
                        throw new DeployArgumentsException();
                    }
                    string value = command.GetSingleValueArgument(Constants.EngineerVersionArgumentKey);
                    project.Configuration.EngineerVersion = value;
                    return owner.Create(key, value);
                }

                return owner.Create(key, project.Configuration.EngineerVersion);
            }

            Entity GetSolutionVersion()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (command.CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)
                    && command.IsCommandArgumentSpecified(Constants.SolutionVersionArgumentKey))
                {
                    if (command.IsCommandArgumentSpecified(Constants.EngineerVersionArgumentKey))
                    {
                        throw new DeployArgumentsException();
                    }
                    string value = command.GetSingleValueArgument(Constants.SolutionVersionArgumentKey);
                    project.Configuration.SolutionVersion = value;
                    return owner.Create(key, value);
                }

                return owner.Create(key, project.Configuration.SolutionVersion);
            }

            Entity GetLibraryDescription()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (command.CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)
                    && command.IsCommandArgumentSpecified(Constants.DescriptionArgumentKey))
                {
                    string value = command.GetSingleValueArgument(Constants.DescriptionArgumentKey);
                    project.Configuration.LibraryDescription = value;
                    return owner.Create(key, value);
                }

                return owner.Create(key, project.Configuration.LibraryDescription);
            }

            Entity GetLibraryVersion()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (command.CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)
                    && command.IsCommandArgumentSpecified(Constants.VersionArgumentKey))
                {
                    string value = command.GetSingleValueArgument(Constants.VersionArgumentKey);
                    project.Configuration.LibraryVersion = value;
                    return owner.Create(key, value);
                }

                return owner.Create(key, project.Configuration.LibraryVersion);
            }

            Entity GetExcludedFiles()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (command.CommandName.Equals("deploy", StringComparison.OrdinalIgnoreCase)
                    && command.IsCommandArgumentSpecified(EntityKeys.ExcludeFilesKey))
                {
                    IEnumerable<string> files = command.GetMultiValueArgument(EntityKeys.ExcludeFilesKey);
                    project.Configuration.ExcludedFiles = files;
                    return owner.Create(key, files.Select(f => owner.Create(key, f)));
                }

                return owner.Create(key, project.Configuration.ExcludedFiles.Select(f => owner.Create(key, f)));
            }
        }
    }
}
