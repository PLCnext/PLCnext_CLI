#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Commands
{
    internal class GetProjectInformationCommand : SyncCommand<GetProjectInformationCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IFileSystem fileSystem;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;

        public GetProjectInformationCommand(ITransactionFactory transactionFactory,
                                          IExceptionHandler exceptionHandler,
                                          ExecutionContext executionContext, 
                                          ICommandResultVisualizer commandResultVisualizer, 
                                          IEntityFactory entityFactory, IFileSystem fileSystem,
                                          ITargetParser targetParser, ISdkRepository sdkRepository)
            : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.fileSystem = fileSystem;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
        }

        protected override CommandResult ExecuteDetailed(GetProjectInformationCommandArgs args, ChangeObservable observable)
        {
            Entity root = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            ProjectEntity project = ProjectEntity.Decorate(root);
            TemplateEntity template = TemplateEntity.Decorate(root);

            string ns = CodeEntity.Decorate(project).Namespace;

            string name = null;
            if (fileSystem.FileExists(System.IO.Path.Combine(root.Path, Constants.ProjectFileName)))
                name = root.Name;

            string type = project.Type;

            TargetsResult targetsResult = targetParser.Targets(project, false);
            Target[] availableTargets = sdkRepository.GetAllTargets().ToArray();
            IEnumerable<Exception> exceptions = Enumerable.Empty<Exception>();
            IEnumerable <TargetResult> commandResult = targetsResult.ValidTargets
                                                     .Select(t => new TargetResult(t.Name, t.Version,
                                                        t.LongVersion,
                                                        t.ShortVersion,
                                                        availableTargets.Any(at => t.Name == at.Name && at.LongVersion == t.LongVersion)));
            exceptions = targetsResult.Errors;

            IEnumerable<CodeEntity> entities = template.EntityHierarchy.Select(e =>
            {
                CodeEntity codeEntity = CodeEntity.Decorate(e);
                return codeEntity;
            }
            );

            IEnumerable<EntityResult> entitiesResult = entities.Select(e =>
            {
                TemplateEntity te = TemplateEntity.Decorate(e);
                return new EntityResult(e.Name, e.Namespace, e.Type, te.RelatedEntites.Where(en => !en.Type.Contains("project")).Select(en => en.Name));
            }).Where(e => !e.Type.Contains("project"));

            return new CommandResult(0, new ProjectInformationCommandResult(name, ns, type, commandResult, entitiesResult), exceptions);
        }
    }
}
