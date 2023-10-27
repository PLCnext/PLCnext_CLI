#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Tools;
using System.Threading.Tasks;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using System;

namespace PlcNext.Common.Commands
{
    internal class CheckProjectCommand : SyncCommand<CheckProjectCommandArgs>
    {
        private readonly IEntityFactory entityFactory;

        public CheckProjectCommand(ITransactionFactory transactionFactory,
                                   IExceptionHandler exceptionHandler,
                                   ExecutionContext executionContext,
                                   ICommandResultVisualizer commandResultVisualizer,
                                   IEntityFactory entityFactory) 
            : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
        }

        protected override int Execute(CheckProjectCommandArgs args, ChangeObservable observable)
        {
            Entity rootEntity = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            ProjectEntity project = ProjectEntity.Decorate(rootEntity);
            if (project.Version.Major > project.ToolProjectVersion.Major)
            {
                throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
            }

            return 0;
        }
    }
}
