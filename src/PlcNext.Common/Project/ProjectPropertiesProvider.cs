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
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using System;
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

        IDictionary<string, bool> IncludePaths { get; }

        void Initialize(Entity root);

        IEnumerable<Exception> Exceptions { get; }
    }

    internal class ProjectPropertiesProvider : IProjectPropertiesProvider
    {
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;
        private readonly IFileSystem fileSystem;

        public ProjectPropertiesProvider(ITargetParser targetParser, ISdkRepository sdkRepository, IFileSystem fileSystem)
        {
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
            this.fileSystem = fileSystem;
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
                Exceptions = Enumerable.Empty<Exception>();

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
                    return (e, te.RelatedEntites.Where(en => !en.Type.Contains("project")));
                })
                    .Where(e => !e.Item1.Type.Contains("project")).ToDictionary(p => p.Item1, p => p.Item2);

            }
            
            void SetProjectIncludes()
            {
                IEnumerable<SdkInformation> relevantSdks = ProjectTargets.Select(t => availableTargets.FirstOrDefault(at => t.Target.Name == at.Name && at.LongVersion == t.Target.LongVersion))
                                             .Where(t => t != null)
                                             .Select(sdkRepository.GetSdk)
                                             .Where(sdk => sdk != null)
                                             .Distinct();
                IncludePaths = relevantSdks.SelectMany(sdk => sdk.IncludePaths)
                                           .Concat(relevantSdks.SelectMany(sdk => sdk.CompilerInformation.IncludePaths))
                                           .Distinct()
                                           .ToDictionary(x => x, x => true);
                
                    foreach (KeyValuePair<string, VirtualDirectory> codeModelIncludeDirectory in codeModel.IncludeDirectories)
                    {
                        if(!IncludePaths.ContainsKey(codeModelIncludeDirectory.Key))
                            IncludePaths.Add(codeModelIncludeDirectory.Key, codeModelIncludeDirectory.Value!=null);
                    }
            }
        }

        public string ProjectName { get; private set; }

        public string ProjectNamespace { get; private set; }

        public string ProjectType { get; private set; }

        public IEnumerable<ProjectTarget> ProjectTargets { get; private set; }

        public Dictionary<CodeEntity, IEnumerable<Entity>> ProjectCodeEntities { get; private set; }

        public IDictionary<string, bool> IncludePaths { get; private set; }

        public IEnumerable<Exception> Exceptions { get; private set; }
    }
}
