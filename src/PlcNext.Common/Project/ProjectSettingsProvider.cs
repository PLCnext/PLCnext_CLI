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
using System.Xml.Serialization;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Project
{
    internal class ProjectSettingsProvider : ITemplateIdentifier, IEntityContentProvider
    {
        private readonly ExecutionContext executionContext;
        public string IdentifierKey => "ProjectSettingsIdentifier";

        private readonly IFileSystem fileSystem;

        private static readonly string[] AvailableProjectValues = new[]
        {
            EntityKeys.PathKey,
            EntityKeys.NameKey,
            EntityKeys.ProjectSettingsKey,
        };

        public ProjectSettingsProvider(IFileSystem fileSystem, ExecutionContext executionContext)
        {
            this.fileSystem = fileSystem;
            this.executionContext = executionContext;
        }
        
        public IEnumerable<Entity> FindAllEnities(string entityName, Entity owner)
        {
            if (!owner.HasPathCommandArgument())
            {
                executionContext.WriteVerbose("The executed command lacks the path argument, no project file can be loaded.");
                return Enumerable.Empty<Entity>();
            }

            string rootFilePath = fileSystem.GetDirectory(owner.GetPathCommandArgument(), createNew: false)
                                            .FullName;

            VirtualFile file = fileSystem.FileExists(rootFilePath)
                                   ? fileSystem.GetFile(rootFilePath)
                                   : fileSystem.FileExists(Constants.ProjectFileName, rootFilePath)
                                       ? fileSystem.GetFile(Constants.ProjectFileName, rootFilePath)
                                       : null;
            if (file == null)
            {
                executionContext.WriteVerbose($"No project file found in {rootFilePath}.");
                return Enumerable.Empty<Entity>();
            }

            try
            {
                using (Stream fileStream = file.OpenRead())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ProjectSettings));
                    ProjectSettings settings = (ProjectSettings)serializer.Deserialize(fileStream);
                    if(settings.Type != null && settings.Type.Equals(entityName))
                        return new[] { owner.Create(entityName, new ProjectDescription(settings, file.Parent, file)) };
                }
            }
            catch (Exception e)
            {
                executionContext.WriteVerbose($"Error while trying to parse project settings {file.FullName}." +
                                                      $"{Environment.NewLine}{e}");
            }

            return Enumerable.Empty<Entity>();
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.IsRoot() &&
                   AvailableProjectValues.Contains(key) &&
                   owner.HasValue<ProjectDescription>();
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.PathKey:
                    return GetProjectPath();
                case EntityKeys.NameKey:
                    return GetProjectName();
                case EntityKeys.ProjectSettingsKey:
                    return GetProjectSettings();
                default:
                    throw new ContentProviderException(key, owner);
            }

            Entity GetProjectPath()
            {
                ProjectDescription description = owner.Value<ProjectDescription>();
                VirtualDirectory path = description?.Root ?? fileSystem.CurrentDirectory;
                return owner.Create(key, path.FullName, path);
            }

            Entity GetProjectSettings()
            {
                ProjectDescription description = owner.Value<ProjectDescription>();
                return owner.Create(key, new MutableProjectSettings(description.Settings, description.File, executionContext));
            }

            Entity GetProjectName()
            {
                ProjectDescription description = owner.Value<ProjectDescription>();
                return owner.Create(key, description?.Root.Name?? Path.GetFileName(owner.Path));
            }
        }

        private class ProjectDescription
        {
            public ProjectDescription(ProjectSettings settings, VirtualDirectory root, VirtualFile file)
            {
                Settings = settings;
                Root = root;
                File = file;
            }

            public ProjectSettings Settings { get; }
            public VirtualDirectory Root { get; }
            public VirtualFile File { get; }
        }
    }
}
