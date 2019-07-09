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
    internal class GetSdksCommand : SyncCommand<GetSdksCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;

        public GetSdksCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, ISdkRepository sdkRepository) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
        }

        protected override CommandResult ExecuteDetailed(GetSdksCommandArgs args, ChangeObservable observable)
        {
            string[] sdks;
            IEnumerable<Exception> exceptions = Enumerable.Empty<Exception>();
            if (args.All)
            {
                sdks = sdkRepository.Sdks.Select(s => s.Root.FullName).ToArray();
            }
            else
            {
                ProjectEntity project = ProjectEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
                TargetsResult targetsResult = targetParser.Targets(project);
                sdks = targetsResult.ValidTargets.Select(sdkRepository.GetSdk).Distinct()
                                          .Select(s => s.Root.FullName).ToArray();

                exceptions = targetsResult.Errors;
            }

            return new CommandResult(0, new SdksCommandResult(sdks.Select(s => new Path(s))), exceptions);
        }
    }
}