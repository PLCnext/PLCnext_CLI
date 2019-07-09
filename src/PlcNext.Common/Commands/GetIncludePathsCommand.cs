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
using System.Linq;
using System.Text;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using IEntityFactory = PlcNext.Common.DataModel.IEntityFactory;

namespace PlcNext.Common.Commands
{
    internal class GetIncludePathsCommand : SyncCommand<GetIncludePathsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;

        public GetIncludePathsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, ISdkRepository sdkRepository) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
        }

        protected override CommandResult ExecuteDetailed(GetIncludePathsCommandArgs args, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            TargetsResult targetsResult = targetParser.Targets(project);
            IEnumerable<string> paths = targetsResult.ValidTargets
                                                     .Select(sdkRepository.GetSdk)
                                                     .Where(sdk => sdk != null)
                                                     .Distinct()
                                                     .SelectMany(sdk => sdk.IncludePaths);

            return new CommandResult(0, new IncludePathsCommandResult(paths.Select(p => new Path(p))),
                                     targetsResult.Errors);
        }
    }
}
