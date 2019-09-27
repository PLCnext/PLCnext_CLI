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
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class DynamicCommand : AsyncCommand<DynamicCommandArgs>
    {
        private readonly IEnumerable<IDynamicCommandProvider> commandProviders;

        public DynamicCommand(ITransactionFactory transactionFactory,
                              IExceptionHandler exceptionHandler, IEnumerable<IDynamicCommandProvider> commandProviders,
                              ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer) : base(
            transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.commandProviders = commandProviders;
        }

        protected override async Task<int> ExecuteAsync(DynamicCommandArgs args, ChangeObservable observable)
        {
            IDynamicCommandProvider commandProvider = commandProviders.First(p => p.CanExecute(args.Definition));
            return await commandProvider.Execute(args.Definition, observable).ConfigureAwait(false);
        }
    }
}
