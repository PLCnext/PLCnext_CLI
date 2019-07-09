#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class GetAppComponentsCommand : SyncCommand<GetAppComponentsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;

        public GetAppComponentsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
        }

        protected override CommandResult ExecuteDetailed(GetAppComponentsCommandArgs args, ChangeObservable observable)
        {
            TemplateEntity project = TemplateEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            IEnumerable<Entity> components = project.EntityHierarchy.Where(e => e.Type.Equals("appcomponent", StringComparison.OrdinalIgnoreCase))
                                                    .ToArray();
            if (!components.Any())
            {
                ExecutionContext.WriteInformation($"No appcomponents found for project {project.Name}");
                return new CommandResult(0, ComponentsCommandResult.Empty);
            }

            return new CommandResult(0, new ComponentsCommandResult(components.Select(c =>
            {
                CodeEntity codeEntity = CodeEntity.Decorate(c);
                return new ComponentResult(codeEntity.Name, codeEntity.Namespace);
            })));
        }
    }
}
