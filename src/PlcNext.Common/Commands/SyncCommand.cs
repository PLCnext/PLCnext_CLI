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
using System.Threading.Tasks;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal abstract class SyncCommand<T> : Command<T>
        where T : CommandArgs
    {
        protected SyncCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, bool hasDetailedResult = false) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, false, hasDetailedResult)
        {
        }
    }
}
