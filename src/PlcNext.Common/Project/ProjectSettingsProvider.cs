﻿#region Copyright
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
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.Project
{
    internal class ProjectSettingsProvider : PriorityContentProvider, ITemplateIdentifier
    {
        private readonly ExecutionContext executionContext;
        private readonly IGuidFactory guidFactory;
        private readonly Version toolProjectVersion = new(4, 0);
        public string IdentifierKey => "ProjectSettingsIdentifier";

        private readonly IFileSystem fileSystem;

        private static readonly string[] AvailableProjectValues = new[]
        {
            EntityKeys.PathKey,
            EntityKeys.NameKey,
            EntityKeys.ProjectSettingsKey,
            EntityKeys.ProjectVersionKey,
            EntityKeys.GenerateDTArrayNameByTypeKey
        };

        public override SubjectIdentifier HigherPrioritySubject { get; } = new SubjectIdentifier(nameof(CommandDefinitionContentProvider));

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public ProjectSettingsProvider(IFileSystem fileSystem, ExecutionContext executionContext, IGuidFactory guidFactory)
        {
            this.fileSystem = fileSystem;
            this.executionContext = executionContext;
            this.guidFactory = guidFactory;
        }

        public IEnumerable<Entity> FindAllEntities(string entityName, Entity owner)
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
                using (XmlReader reader = XmlReader.Create(fileStream))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ProjectSettings));
                    ProjectSettings settings = (ProjectSettings)serializer.Deserialize(reader);
                    if(settings.Type != null && settings.Type.Equals(entityName, StringComparison.Ordinal))
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

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.IsRoot() &&
                   ((AvailableProjectValues.Contains(key) &&
                     owner.HasValue<ProjectDescription>()) ||
                    key == EntityKeys.ProjectIdKey ||
                    key == EntityKeys.GenerateDTArrayNameByTypeKey) ||
                    key == EntityKeys.GenerateDatatypeNamespaces ||
                   key == EntityKeys.CSharpProjectPath||
                   key == EntityKeys.ToolProjectVersionKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.PathKey:
                    return GetProjectPath();
                case EntityKeys.NameKey:
                    return GetProjectName();
                case EntityKeys.ProjectSettingsKey:
                    return GetProjectSettings();
                case EntityKeys.ProjectVersionKey:
                    return GetProjectVersion();
                case EntityKeys.ToolProjectVersionKey:
                    return GetToolProjectVersion();
                case EntityKeys.ProjectIdKey:
                    return GetProjectId();
                case EntityKeys.GenerateDTArrayNameByTypeKey:
                    return GetGenerateDTArrayNameByTypeFlag();
                case EntityKeys.CSharpProjectPath:
                    return GetCSharpProjectPath();
                case EntityKeys.GenerateDatatypeNamespaces:
                    return GetGenerateNamespacesFlag();
                default:
                    throw new ContentProviderException(key, owner);
            }

            Entity GetProjectId()
            {
                CommandEntity command = CommandEntity.Decorate(owner.Origin);
                if (command.IsCommandArgumentSpecified(Constants.IdArgumentName))
                {
                    string id = command.GetSingleValueArgument(Constants.IdArgumentName);
                    if (!Guid.TryParse(id, out Guid guid))
                    {
                        throw new LibraryIdMalformattedException(id);
                    }

                    return owner.Create(key, guid, guid.ToString("D", CultureInfo.InvariantCulture));
                }

                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (!project.Settings.IsPersistent)
                {
                    executionContext.WriteWarning("The id for the library will change for each generation please use the --id option to set the id.");
                    Guid id = guidFactory.Create();
                    return owner.Create(key, id, id.ToString("D", CultureInfo.InvariantCulture));
                }

                string storedId = project.Settings.Value.Id;
                if (string.IsNullOrEmpty(storedId))
                {
                    storedId = guidFactory.Create().ToString("D", CultureInfo.InvariantCulture);
                    project.Settings.SetId(storedId);
                }

                Guid result = Guid.Parse(storedId);
                return owner.Create(key, result, result.ToString("D", CultureInfo.InvariantCulture));
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
                string name = description?.Settings.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = Path.GetFileName(owner.Path);
                }
                return owner.Create(key, name);
            }

            Entity GetProjectVersion()
            {
                ProjectDescription description = owner.Value<ProjectDescription>();
                return owner.Create(key, Version.Parse(description.Settings.Version));
            }
            
            Entity GetToolProjectVersion()
            {
                return owner.Create(key, toolProjectVersion.ToString(), toolProjectVersion);
            }

            Entity GetGenerateDTArrayNameByTypeFlag()
            {
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (!project.Settings.IsPersistent)
                {
                    return owner.Create(key, "true", true); //always generate new array name schema if no .proj available
                }
                ProjectDescription description = owner.Value<ProjectDescription>();
                return owner.Create(key, description.Settings.GenerateDTArrayNameByType.ToString(CultureInfo.InvariantCulture),
                                         description.Settings.GenerateDTArrayNameByType);
            }

            Entity GetCSharpProjectPath()
            {
                ProjectEntity project = ProjectEntity.Decorate(owner);
                if (project.Settings.IsPersistent)
                {
                    ProjectDescription description = owner.Value<ProjectDescription>();
                    return owner.Create(key, description.Settings.CSharpProjectPath);
                }
                //do not allow shared native projects without .proj file
                throw new ContentProviderException(key, owner);
            }

            Entity GetGenerateNamespacesFlag()
            {
                ProjectEntity project = ProjectEntity.Decorate(owner.Root);

                if (!project.Settings.IsPersistent)
                {
                    return owner.Create(key, true.ToString(CultureInfo.InvariantCulture), true);
                }
                if (project.Settings.Value.GenerateNamespacesSpecified)
                {
                    return owner.Create(key, project.Settings.Value.GenerateNamespaces.ToString(CultureInfo.InvariantCulture), project.Settings.Value.GenerateNamespaces);
                }

                return owner.Create(key, false.ToString(CultureInfo.InvariantCulture), false);

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
