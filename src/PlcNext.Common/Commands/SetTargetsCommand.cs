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
using PlcNext.Common.Project;
using PlcNext.Common.Project.Persistence;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using IEntityFactory = PlcNext.Common.DataModel.IEntityFactory;

namespace PlcNext.Common.Commands
{
    internal class SetTargetsCommand : SyncCommand<SetTargetsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly ITargetParser targetParser;
        private readonly IUserInterface userInterface;

        public SetTargetsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.userInterface = userInterface;
        }

        protected override int Execute(SetTargetsCommandArgs args, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            if (project.Version.Major > project.ToolProjectVersion.Major)
            {
                throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
            }
            
            FileEntity fileEntity = FileEntity.Decorate(project);
            if (!project.Settings.IsPersistent)
                throw new TargetNotSettableForProjectException();

            if (args.Add)
            {
                Target result = targetParser.AddTarget(project, args.Name, args.Version);
                userInterface.WriteInformation($"Successfully added target {result.Name} {result.LongVersion} to project {fileEntity.Directory.FullName}.");
            }
            else if (args.Remove)
            {
                Target result = targetParser.RemoveTarget(project, args.Name, args.Version);
                userInterface.WriteInformation($"Successfully removed target {result.Name} {result.LongVersion} from project {fileEntity.Directory.FullName}.");
            }
            else
            {
                throw new SetTargetsOptionMissingException();
            }

            return 0;
        }
    }
}
