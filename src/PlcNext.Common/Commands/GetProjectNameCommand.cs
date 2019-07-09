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
using System.Text;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;
using Path = System.IO.Path;

namespace PlcNext.Common.Commands
{
    internal class GetProjectNameCommand : SyncCommand<GetProjectNameCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IFileSystem fileSystem;

        public GetProjectNameCommand(ITransactionFactory transactionFactory, 
                                     IExceptionHandler exceptionHandler,
                                     ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, IFileSystem fileSystem) 
            : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.fileSystem = fileSystem;
        }

        protected override CommandResult ExecuteDetailed(GetProjectNameCommandArgs args, ChangeObservable observable)
        {
            Entity root = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            if (!fileSystem.FileExists(Path.Combine(root.Path, Constants.ProjectFileName)))
                throw new FormattableException($"This command is only available for projects with {Constants.ProjectFileName} file.");
            
            return new CommandResult(0,new NameCommandResult(root.Name));
        }
    }
}
