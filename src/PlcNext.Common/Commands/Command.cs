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
    internal abstract class Command<T> : ICommand
        where T : CommandArgs
    {
        private readonly ITransactionFactory transactionFactory;
        private readonly IExceptionHandler exceptionHandler;
        protected readonly ExecutionContext ExecutionContext;
        private readonly ICommandResultVisualizer commandResultVisualizer;
        private readonly bool executeAsync;

        protected Command(ITransactionFactory transactionFactory,
                          IExceptionHandler exceptionHandler,
                          ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer,
                          bool executeAsync)
        {
            this.transactionFactory = transactionFactory;
            this.exceptionHandler = exceptionHandler;
            this.ExecutionContext = executionContext;
            this.commandResultVisualizer = commandResultVisualizer;
            this.executeAsync = executeAsync;
        }

        public Type CommandArgsType => typeof(T);

        public async Task<int> Execute(CommandArgs args)
        {
            int result = -1;
            bool visualized = false;
            using (ITransaction transaction = transactionFactory.StartTransaction(out ChangeObservable observable))
            using (ExecutionContext.RegisterObservable(observable))
            {
                try
                {
                    CommandResult commandResult = executeAsync
                                                      ? await ExecuteDetailedAsync((T) args, observable).ConfigureAwait(false)
                                                      : await Task.Run(() => ExecuteDetailed((T) args, observable)).ConfigureAwait(false);
                    result = commandResult.ExternalResult;
                    if (commandResult.DetailedResult != null)
                    {
                        commandResultVisualizer.Visualize(commandResult.DetailedResult, args);
                        visualized = true;
                    }

                    commandResult.Exceptions.ThrowIfNotEmpty();
                    transaction.OnCompleted();
                }
                catch (Exception e)
                {
                    if (!exceptionHandler.HandleException(e))
                    {
                        throw;
                    }

                    result = -1;
                }
                finally
                {
                    if (!visualized && args.Deprecated)
                    {
                        ExecutionContext.WriteWarning($"This command is deprecated. Please use '{args.DeprecatedAlternative}' instead.");
                    }
                }
            }


            return result;
        }

        protected virtual int Execute(T args, ChangeObservable observable)
        {
            throw new InvalidOperationException("Either Execute or ExecuteDetailed must be implemented.");
        }

        protected virtual CommandResult ExecuteDetailed(T args, ChangeObservable observable)
        {
            return new CommandResult(Execute(args, observable), null);
        }

        protected virtual Task<int> ExecuteAsync(T args, ChangeObservable observable)
        {
            throw new InvalidOperationException("Either ExecuteAsync or ExecuteDetailedAsync must be implemented.");
        }

        protected virtual async Task<CommandResult> ExecuteDetailedAsync(T args, ChangeObservable observable)
        {
            return new CommandResult(await ExecuteAsync(args, observable).ConfigureAwait(false), null);
        }
    }
}
