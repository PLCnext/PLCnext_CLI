#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Build;
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Project
{
    internal interface IProjectPropertiesProvider
    {
        string ProjectName { get; }

        string ProjectNamespace { get; }

        string ProjectType { get; }

        IEnumerable<ProjectTarget> ProjectTargets { get; }

        Dictionary<CodeEntity, IEnumerable<Entity>> ProjectCodeEntities { get; }

        List<IncludePath> IncludePaths { get; }

        IEnumerable<string> ExternalLibraries { get; }

        bool GenerateNamespaces { get; }

        void Initialize(Entity root);

        IEnumerable<Exception> Exceptions { get; }
    }

    internal class ProjectPropertiesProvider : IProjectPropertiesProvider
    {
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;
        private readonly IFileSystem fileSystem;
        private readonly ExecutionContext executionContext;

        public ProjectPropertiesProvider(ITargetParser targetParser, ISdkRepository sdkRepository, IFileSystem fileSystem, ExecutionContext context)
        {
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
            this.fileSystem = fileSystem;
            this.executionContext = context;
        }

        public void Initialize(Entity root)
        {
            ProjectEntity project = ProjectEntity.Decorate(root);
            TemplateEntity template = TemplateEntity.Decorate(root);
            Target[] availableTargets = sdkRepository.GetAllTargets().ToArray();
            ICodeModel codeModel = root.Value<ICodeModel>();

            SetProjectName();
            SetProjectNamespace();
            SetProjectType();
            SetProjectTargets();
            SetProjectEntities();
            SetProjectIncludes();
            SetExternalLibraries();
            SetGenerateNamespaces();

            void SetProjectName()
            {
                ProjectName = null;
                if (fileSystem.FileExists(System.IO.Path.Combine(root.Path, Constants.ProjectFileName)))
                    ProjectName = root.Name;
            }

            void SetProjectNamespace()
            {
                ProjectNamespace = CodeEntity.Decorate(project).Namespace;
            }

            void SetProjectType()
            {
                ProjectType = project.Type;
            }

            void SetProjectTargets()
            {
                TargetsResult targetsResult = targetParser.Targets(project, false);

                ProjectTargets = targetsResult.ValidTargets
                    .Select(t => new ProjectTarget(t, availableTargets.Any(at => t.Name == at.Name && at.LongVersion == t.LongVersion)));

                Exceptions = targetsResult.Errors;
            }

            void SetProjectEntities()
            {
                IEnumerable<CodeEntity> entities = template.EntityHierarchy.Select(e =>
                {
                    CodeEntity codeEntity = CodeEntity.Decorate(e);
                    return codeEntity;
                }
                );

                ProjectCodeEntities = entities.Select(e =>
                {
                    TemplateEntity te = TemplateEntity.Decorate(e);
                    return (e, te.RelatedEntites.Where(en => !en.Type.Contains("project", StringComparison.Ordinal)));
                })
                    .Where(e => !e.Item1.Type.Contains("project", StringComparison.Ordinal)).ToDictionary(p => p.Item1, p => p.Item2);

            }

            void SetProjectIncludes()
            {
                IncludePaths = new List<IncludePath>();
                IEnumerable<SdkInformation> relevantSdks = ProjectTargets.Select(t => availableTargets.FirstOrDefault(at => t.Target.Name == at.Name && at.LongVersion == t.Target.LongVersion))
                                             .Where(t => t != null)
                                             .Select(sdkRepository.GetSdk)
                                             .Where(sdk => sdk != null)
                                             .Distinct();
                var targetsWithIncludePaths = relevantSdks.Select(sdk => (sdk.Targets, sdk.IncludePaths.Concat(sdk.CompilerInformation.IncludePaths).Distinct()));

                foreach (var item in targetsWithIncludePaths)
                {
                    foreach (Target target in item.Targets)
                    {
                        foreach (string includePath in item.Item2)
                        {
                            IncludePath existingIncludePath = IncludePaths.Where(i => i.PathValue.Equals(includePath, StringComparison.Ordinal)).FirstOrDefault();

                            if (existingIncludePath == null)
                            {
                                existingIncludePath = new IncludePath(includePath, true, new List<Target>());
                                IncludePaths.Add(existingIncludePath);
                            }
                            existingIncludePath.Targets = existingIncludePath.Targets.Concat(new[] { target });
                        }
                    }
                }
                
                foreach (IncludePath codeModelIncludeDirectory in codeModel.IncludeDirectories)
                {
                    IncludePath existingIncludePath = IncludePaths.Where(p => p.PathValue.Equals(codeModelIncludeDirectory.PathValue, StringComparison.Ordinal)).FirstOrDefault();

                    if (existingIncludePath == null)
                    {
                        IncludePaths.Add(codeModelIncludeDirectory);
                    }
                    else
                    {
                        foreach (Target target in codeModelIncludeDirectory.Targets)
                        {
                            if (!existingIncludePath.Targets.Contains(target))
                            {
                                existingIncludePath.Targets = existingIncludePath.Targets.Concat(new[] { target });
                            }
                        }
                    }  
                }
            }

            void SetExternalLibraries()
            {
                if (project.Targets.Any())
                {
                    BuildEntity buildEntity = BuildEntity.Decorate(project.Targets.First());
                    if (buildEntity.HasBuildSystem)
                    {
                        try
                        {
                            ExternalLibraries = buildEntity.BuildSystem.ExternalLibraries;
                            return;
                        }
                        catch (FormattableException)
                        {
                            executionContext.WriteError("External libraries could not be fetched" );
                        }
                    }
                }
                ExternalLibraries = Enumerable.Empty<string>();
            }

            void SetGenerateNamespaces()
            {
                GenerateNamespaces = project.GenerateNamespaces;
            }
        }

        public string ProjectName { get; private set; }

        public string ProjectNamespace { get; private set; }

        public string ProjectType { get; private set; }

        public IEnumerable<ProjectTarget> ProjectTargets { get; private set; }

        public Dictionary<CodeEntity, IEnumerable<Entity>> ProjectCodeEntities { get; private set; }

        public List<IncludePath> IncludePaths { get; private set; }

        public IEnumerable<string> ExternalLibraries { get; private set; }

        public bool GenerateNamespaces { get; private set; }

        public IEnumerable<Exception> Exceptions { get; private set; }
    }
}
