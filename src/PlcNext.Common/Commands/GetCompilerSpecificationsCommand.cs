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
    internal class GetCompilerSpecificationsCommand : SyncCommand<GetCompilerSpecificationsCommandArgs>
    {
        private readonly IEntityFactory entityFactory;
        private readonly ITargetParser targetParser;
        private readonly ISdkRepository sdkRepository;

        public GetCompilerSpecificationsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, IEntityFactory entityFactory, ITargetParser targetParser, ISdkRepository sdkRepository) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.entityFactory = entityFactory;
            this.targetParser = targetParser;
            this.sdkRepository = sdkRepository;
        }

        protected override CommandResult ExecuteDetailed(GetCompilerSpecificationsCommandArgs args, ChangeObservable observable)
        {
            ProjectEntity project = ProjectEntity.Decorate(entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root);
            TargetsResult targetsResult = targetParser.Targets(project);
            IEnumerable<Compiler> compilers = targetsResult.ValidTargets
                                                     .Select(sdkRepository.GetSdk)
                                                     .Where(sdk => sdk != null)
                                                     .Distinct()
                                                     .Select(sdk => sdk.Compiler);

            return new CommandResult(0,
                                     new CompilerSpecificationCommandResult(
                                         compilers.Select(c => new CompilerSpecificationResult(c.CompilerPath,
                                                                                               "cpp",
                                                                                               c.Sysroot,
                                                                                               c.Flags,
                                                                                               c.IncludePaths.Select(p => new Path(p)),
                                                                                               c.Makros.Select(m => new CompilerMacroResult(m.Name, m.Value))))),
                targetsResult.Errors);
        }
    }
}
