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
using System.Diagnostics;
using System.Linq;
using System.Text;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools
{
    internal class FormattableExceptionHandler : IExceptionHandler
    {
        private readonly ExecutionContext executionContext;

        public FormattableExceptionHandler(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public bool HandleException(Exception e)
        {
            if (e is FormattableException formattableException)
            {
                WriteError(formattableException);
                return true;
            }

            if (e is AggregateException aggregateException &&
                aggregateException.InnerExceptions.All(IsHandlable))
            {
                foreach (Exception exception in aggregateException.InnerExceptions.Distinct())
                {
                    HandleException(exception);
                }
                return true;
            }

            return false;

            void WriteError(FormattableException exception)
            {
                executionContext.WriteError(exception.Message);
                Exception inner = exception.InnerException;
                while (inner != null)
                {
                    executionContext.WriteError($"-> {inner.Message}");
                    inner = inner.InnerException;
                }
                executionContext.WriteError($"Complete exception: {exception}", false);
            }

            bool IsHandlable(Exception exception)
            {
                return exception is FormattableException || 
                       (exception is AggregateException aggregate &&
                        aggregate.InnerExceptions.All(IsHandlable));
            }
        }
    }
}
