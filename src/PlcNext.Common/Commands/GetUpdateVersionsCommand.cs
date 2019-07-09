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
using System.Threading.Tasks;
using PlcNext.Common.Installation;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class GetUpdateVersionsCommand : AsyncCommand<GetUpdateVersionsCommandArgs>
    {
        private readonly ICliUpdater cliUpdater;
        private readonly IUserInterface userInterface;

        public GetUpdateVersionsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ICliUpdater cliUpdater, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext,commandResultVisualizer)
        {
            this.cliUpdater = cliUpdater;
            this.userInterface = userInterface;
        }

        protected override async Task<int> ExecuteAsync(GetUpdateVersionsCommandArgs commandArgs, ChangeObservable observable)
        {
            IEnumerable<Version> versions = await cliUpdater.GetAvailableVersions();
            userInterface.WriteInformation("The following versions are available:");
            userInterface.WriteInformation(string.Join(Environment.NewLine, versions.Select(v => v.ToString(3))));
            return 0;
        }
    }
}
