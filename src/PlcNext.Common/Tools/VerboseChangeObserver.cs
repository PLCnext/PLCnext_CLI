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
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools
{
    internal class VerboseChangeObserver : IObserver<Change>, IDisposable
    {
        private readonly ExecutionContext context;
        private readonly IDisposable subscriptionToken;
        private VerboseChangeObserver observer;
        
        public VerboseChangeObserver(ExecutionContext context)
        {
            this.context = context;
            context.ObservableRegistered += ContextOnObservableRegistered;
            context.ObservableUnregistered += ContextOnObservableUnregistered;
        }

        private void ContextOnObservableUnregistered(object sender, EventArgs e)
        {
            observer?.Dispose();
        }

        private void ContextOnObservableRegistered(object sender, EventArgs e)
        {
            observer = new VerboseChangeObserver(context, context.Observable);
        }

        private VerboseChangeObserver(ExecutionContext context, ChangeObservable observable)
        {
            this.context = context;
            subscriptionToken = observable.Subscribe(this);
        }

        public void OnCompleted()
        {
            context.WriteVerbose("Changes completed.");
        }

        public void OnError(Exception error)
        {
            context.WriteVerbose($"Exception during execution {error}");
        }

        public void OnNext(Change value)
        {
            string message = value.ToString();
            if (!string.IsNullOrEmpty(message))
                context.WriteVerbose(message);
        }

        public void Dispose()
        {
            subscriptionToken?.Dispose();
            observer?.Dispose();
        }
    }
}
