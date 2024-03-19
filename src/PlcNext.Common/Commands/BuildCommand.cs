#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Build;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class BuildCommand : SyncCommand<BuildCommandArgs>
    {
        private readonly IIndex<string, IBuilder> builders;
        private readonly IEntityFactory entityFactory;

        public BuildCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IIndex<string, IBuilder> builders, IEntityFactory entityFactory) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.builders = builders;
            this.entityFactory = entityFactory;
        }

        protected override int Execute(BuildCommandArgs args, ChangeObservable observable)
        {
            string buildProperties = string.Join(" ", args.BuildProperties.Select(Unescape));

            Entity rootEntity = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            ProjectEntity project = ProjectEntity.Decorate(rootEntity);
            if (project.Version.Major > project.ToolProjectVersion.Major)
            {
                throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
            }
            
            TemplateEntity templateEntity = TemplateEntity.Decorate(rootEntity);

            BuildInformation buildInfo = new BuildInformation(rootEntity, args.Configure, args.NoConfigure, buildProperties, args.Output);
            builders[templateEntity.Template.buildEngine].Build(buildInfo, observable, args.Target);

            return 0;

            string Unescape(string arg)
            {
                return arg.Replace("%22","\"", StringComparison.Ordinal);
            }
        }
    }
}
