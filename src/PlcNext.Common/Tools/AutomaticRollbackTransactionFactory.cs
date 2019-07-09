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
using System.Text;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Tools
{
    internal class AutomaticRollbackTransactionFactory : ITransactionFactory
    {
        public ITransaction StartTransaction(out ChangeObservable observable)
        {
            observable = new ChangeObservable();
            return new AutomaticRollbackTransaction(observable);
        }

        private class AutomaticRollbackTransaction : IObserver<Change>, ITransaction
        {
            private readonly IDisposable subscribeToken;
            private bool completed;
            private readonly List<Change> changes = new List<Change>();

            public AutomaticRollbackTransaction(IObservable<Change> observable)
            {
                subscribeToken = observable.Subscribe(this);
            }

            public void OnError(Exception error)
            {
                Rollback();
            }

            public void OnNext(Change value)
            {
                changes.Add(value);
            }

            public void OnCompleted()
            {
                completed = true;
            }

            public void Dispose()
            {
                subscribeToken?.Dispose();
                if (!completed)
                {
                    Rollback();
                }
            }

            private void Rollback()
            {
                Trace.TraceInformation("Rollback transaction.");
                changes.Reverse();
                foreach (Change change in changes)
                {
                    change.Invert();
                }
            }
        }
    }
}
