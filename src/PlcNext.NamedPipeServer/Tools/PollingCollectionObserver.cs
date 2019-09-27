#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Linq;

namespace PlcNext.NamedPipeServer.Tools
{
    internal class PollingCollectionObserver : IDisposable
    {
        private readonly ICollection[] observedCollections;
        private readonly Action executedAction;
        private readonly HighResolutionTimer timer;

        private PollingCollectionObserver(ICollection[] observedCollections, Action executedAction)
        {
            this.observedCollections = observedCollections;
            this.executedAction = executedAction;
            timer = new HighResolutionTimer(20) {UseHighPriorityThread = false};
            timer.Elapsed += TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
        {
            if (observedCollections.Any(c => c.Count > 0))
            {
                executedAction();
            }
        }

        public static PollingCollectionObserver Observe(Action executedAction, params ICollection[] observedCollections)
        {
            PollingCollectionObserver observer = new PollingCollectionObserver(observedCollections, executedAction);
            observer.StartPolling();
            return observer;
        }

        private void StartPolling()
        {
            timer.Start();
        }

        public void Dispose()
        {
            timer.Elapsed -= TimerOnElapsed;
            timer.Stop(false);
        }
    }
}