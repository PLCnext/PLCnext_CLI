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
    internal class GetComponentsCommand : SyncCommand<GetComponentsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;

        public GetComponentsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.entityFactory = entityFactory;
        }

        protected override CommandResult ExecuteDetailed(GetComponentsCommandArgs args, ChangeObservable observable)
        {
            ExecutionContext.WriteWarning("This command is deprecated. Use 'get project-information' instead.", false);

            TemplateEntity project = TemplateEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            IEnumerable<Entity> components = project.EntityHierarchy.Where(e => e.Type.Equals("component",StringComparison.OrdinalIgnoreCase))
                                                    .ToArray();
            if (!components.Any())
            {
                ExecutionContext.WriteInformation($"No components found for project {project.Name}");
                return new CommandResult(0, ComponentsCommandResult.Empty);
            }

            return new CommandResult(0, new ComponentsCommandResult(components.Select(c =>
            {
                CodeEntity codeEntity = CodeEntity.Decorate(c);
                return new ComponentResult(c.Name,
                                           string.IsNullOrEmpty(codeEntity.Namespace)
                                               ? string.Empty
                                               : codeEntity.Namespace);
            })));
        }
    }
}
