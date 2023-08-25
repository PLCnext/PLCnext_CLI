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
using System.Linq;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Project
{
    public class ProjectEntity : EntityBaseDecorator
    {
        private ProjectEntity(IEntityBase entityBase) : base(entityBase)
        {
        }

        public static ProjectEntity Decorate(IEntityBase rootEntity)
        {
            if (rootEntity == null)
            {
                throw new ArgumentNullException(nameof(rootEntity));
            }

            if (!rootEntity.IsRoot())
            {
                throw new InvalidOperationException("This decorator can only decorate root entities.");
            }

            return Decorate<ProjectEntity>(rootEntity) ?? new ProjectEntity(rootEntity);
        }

        public MutableProjectSettings Settings => HasValue<MutableProjectSettings>()
                                                      ? Value<MutableProjectSettings>()
                                                      : HasContent(EntityKeys.ProjectSettingsKey)
                                                          ? this[EntityKeys.ProjectSettingsKey]
                                                             .Value<MutableProjectSettings>()
                                                          : new MutableProjectSettings();

        public Version Version => HasValue<Version>()
            ? Value<Version>()
            : HasContent(EntityKeys.ProjectVersionKey)
                ? this[EntityKeys.ProjectVersionKey].Value<Version>()
                : new Version(1, 0);

        public Version ToolProjectVersion => HasContent(EntityKeys.ToolProjectVersionKey)
                ? this[EntityKeys.ToolProjectVersionKey].Value<Version>()
                : new Version(1, 0);

        public IEnumerable<Entity> Targets => HasContent(EntityKeys.TargetsKey)
                                                  ? this[EntityKeys.TargetsKey]
                                                  : Enumerable.Empty<Entity>();

        public IEnumerable<Entity> ValidatedTargets => HasContent(EntityKeys.ValidatedTargetsKey)
                                                  ? this[EntityKeys.ValidatedTargetsKey]
                                                  : Enumerable.Empty<Entity>();

        public Guid Id => this[EntityKeys.ProjectIdKey].Value<Guid>();

        public string LibraryDescription => this[EntityKeys.LibraryDescriptionKey].Value<string>();

        public string LibraryVersion => this[EntityKeys.LibraryVersionKey].Value<string>();

        public string EngineerVersion => this[EntityKeys.EngineerVersionKey].Value<string>();

        public string SolutionVersion => this[EntityKeys.SolutionVersionKey].Value<string>();

        public ProjectConfigurations Configuration => HasValue<ProjectConfigurations>()
                                                        ? Value<ProjectConfigurations>()
                                                        : HasContent(EntityKeys.ProjectConfigurationsKey)
                                                            ? this[EntityKeys.ProjectConfigurationsKey]
                                                             .Value<ProjectConfigurations>()
                                                          : new ProjectConfigurations();
        public IEnumerable<string> ExcludedFiles => this[EntityKeys.ExcludeFilesKey].Select(e => e.Value<string>());

        public string CSharpProjectPath => this[EntityKeys.CSharpProjectPath].Value<string>();

        public bool GenerateNamespaces => this[EntityKeys.GenerateDatatypeNamespaces].Value<bool>();
    }
}
