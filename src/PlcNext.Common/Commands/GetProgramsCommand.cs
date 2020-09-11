#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class GetProgramsCommand : SyncCommand<GetProgramsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;

        public GetProgramsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.entityFactory = entityFactory;
        }

        protected override CommandResult ExecuteDetailed(GetProgramsCommandArgs args, ChangeObservable observable)
        {
            ExecutionContext.WriteWarning("This command is deprecated. Use 'get project-information' instead.", false);

            TemplateEntity project = TemplateEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            IEnumerable<Entity> programs = project.EntityHierarchy.Where(e => e.Type.Equals("program", StringComparison.OrdinalIgnoreCase))
                                                  .ToArray();
            if (!string.IsNullOrEmpty(args.Component))
            {
                Entity component = project.EntityHierarchy.FirstOrDefault(e => e.HasName && e.Name == args.Component &&
                                                                               e.Type.Equals("component", StringComparison.OrdinalIgnoreCase));
                if(component != null)
                {
                    programs = programs.Where(p => TemplateEntity.Decorate(p).RelatedEntites.Contains(component));
                }
                else
                {
                    ExecutionContext.WriteError($"A component with name {args.Component} does not exist in project {project.Name}.");
                    return new CommandResult(-1, ProgramsCommandResult.Empty);
                }
            }
            if (!programs.Any())
            {
                ExecutionContext.WriteInformation($"No programs found.");
                return new CommandResult(0, ProgramsCommandResult.Empty);
            }

            return new CommandResult(0, new ProgramsCommandResult(programs.Select(p =>
            {
                CodeEntity codeEntity = CodeEntity.Decorate(p);
                TemplateEntity templateEntity = TemplateEntity.Decorate(p);
                Entity componentEntity = templateEntity.RelatedEntites.FirstOrDefault(e => e.Type == "component");
                if (componentEntity == null)
                {
                    throw new InvalidOperationException($"Program {p.Name} has no related component. This should not be possible.");
                }

                CodeEntity componentCodeEntity = CodeEntity.Decorate(componentEntity);
                return new ProgramResult(p.Name, codeEntity.Namespace, componentEntity.Name,
                                         componentCodeEntity.Namespace);
            })));
        }
    }
}
