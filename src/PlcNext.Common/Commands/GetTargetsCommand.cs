#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
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
    internal class GetTargetsCommand : SyncCommand<GetTargetsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly IUserInterface userInterface;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;

        public GetTargetsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, ISdkRepository sdkRepository, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
            this.userInterface = userInterface;
        }

        protected override CommandResult ExecuteDetailed(GetTargetsCommandArgs args, ChangeObservable observable)
        {
            TargetsCommandResult commandResult = new TargetsCommandResult(sdkRepository.GetAllTargets()
                                                                    .Select(t => new TargetResult(t.Name, t.Version,
                                                                                                  t.LongVersion,
                                                                                                  t.ShortVersion))
                                                                    .OrderBy(t => t.Name).ThenByDescending(t => t.Version));
            return new CommandResult(0, commandResult);
        }
    }
}