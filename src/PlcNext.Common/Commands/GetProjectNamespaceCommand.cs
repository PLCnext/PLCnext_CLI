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
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Commands.CommandResults;

namespace PlcNext.Common.Commands
{
    internal class GetProjectNamespaceCommand : SyncCommand<GetProjectNamespaceCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IFileSystem fileSystem;

        public GetProjectNamespaceCommand(ITransactionFactory transactionFactory,
                                          IExceptionHandler exceptionHandler,
                                          ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, IFileSystem fileSystem)
            : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.fileSystem = fileSystem;
        }

        protected override CommandResult ExecuteDetailed(GetProjectNamespaceCommandArgs args, ChangeObservable observable)
        {
            Entity root = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            string ns = CodeEntity.Decorate(ProjectEntity.Decorate(root)).Namespace;

            return new CommandResult(0, new ProjectNamespaceCommandResult(ns));
        }
    }
}
