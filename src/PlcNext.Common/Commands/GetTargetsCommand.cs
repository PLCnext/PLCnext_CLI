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

        public GetTargetsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, ISdkRepository sdkRepository, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
            this.userInterface = userInterface;
        }

        protected override CommandResult ExecuteDetailed(GetTargetsCommandArgs args, ChangeObservable observable)
        {
            IEnumerable<Exception> exceptions = Enumerable.Empty<Exception>();
            TargetsCommandResult commandResult;
            if (args.All)
            {
                commandResult = new TargetsCommandResult(sdkRepository.GetAllTargets()
                                                                    .Select(t => new TargetResult(t.Name, t.Version,
                                                                                                  t.LongVersion,
                                                                                                  t.ShortVersion))
                                                                    .OrderBy(t => t.Name).ThenByDescending(t => t.Version));
            }
            else
            {
                ExecutionContext.WriteWarning("This command is deprecated. Use 'get project-information' instead.", false);

                ProjectEntity project = ProjectEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
                TargetsResult targetsResult = targetParser.Targets(project, false);
                Target[] availableTargets = sdkRepository.GetAllTargets().ToArray();
                commandResult = new TargetsCommandResult(targetsResult.ValidTargets
                                                                    .Select(t => new TargetResult(t.Name, t.Version,
                                                                                                  t.LongVersion,
                                                                                                  t.ShortVersion,
                                                                                                  availableTargets.Any(at => t.Name == at.Name && at.LongVersion == t.LongVersion)))
                                                                    .OrderBy(t => t.Name).ThenByDescending(t => t.Version));
                exceptions = targetsResult.Errors;
            }

            return new CommandResult(0, commandResult, exceptions);
        }
    }
}