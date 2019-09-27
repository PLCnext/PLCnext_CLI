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

namespace PlcNext.Common.Tools.Events
{
    public class ChangeObservable : IObservable<Change>
    {
        private readonly List<IObserver<Change>> observers = new List<IObserver<Change>>();
        private readonly List<Change> unnoticedChanges = new List<Change>();

        public IDisposable Subscribe(IObserver<Change> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            if (unnoticedChanges.Any())
            {
                foreach (Change unnoticedChange in unnoticedChanges)
                {
                    observer.OnNext(unnoticedChange);
                }
                unnoticedChanges.Clear();
            }
            observers.Add(observer);
            return new DisposeAction(() => Unsubscribe(observer));
        }

        private void Unsubscribe(IObserver<Change> observer)
        {
            observers.Remove(observer);
        }

        public void OnNext(Change change)
        {
            if (!observers.Any())
            {
                unnoticedChanges.Add(change);
            }
            else
            {
                foreach (IObserver<Change> observer in observers)
                {
                    observer.OnNext(change);
                }
            }
        }
    }
}
