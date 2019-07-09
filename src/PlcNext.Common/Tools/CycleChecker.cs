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
using PlcNext.Common.DataModel;

namespace PlcNext.Common.Tools
{
    public class CycleChecker<T> : IDisposable where T: IEquatable<T>
    {
        private readonly Action resetCycleCheckerInstanceAction;
        private readonly CycleChecker<T> parent;
        private readonly List<T> cycleList;
        private readonly string actionDescription;

        public CycleChecker(string actionDescription, Action resetCycleCheckerInstanceAction = null)
        {
            this.actionDescription = actionDescription;
            this.resetCycleCheckerInstanceAction = resetCycleCheckerInstanceAction;
            cycleList = new List<T>();
        }

        private CycleChecker(CycleChecker<T> parent)
        {
            this.parent = parent;
        }

        public CycleChecker<T> SpawnChild()
        {
            return new CycleChecker<T>(this);
        }

        public void AddItem(T item)
        {
            if (parent != null)
            {
                parent.AddItem(item);
                return;
            }
            bool cycle = cycleList.Contains(item);
            cycleList.Add(item);
            if (cycle)
            {
                string printedCycle = PrintCycle(item);
                throw new CycleException(actionDescription, printedCycle);
            }
        }

        public void RemoveAfter(T item)
        {
            if (parent != null)
            {
                parent.RemoveAfter(item);
                return;
            }

            int index = cycleList.IndexOf(item);
            if (cycleList.Count > index + 1)
            {
                cycleList.RemoveRange(index + 1, cycleList.Count - (index + 1));
            }
        }

        private string PrintCycle(T item)
        {
            StringBuilder builder = new StringBuilder();
            int index = cycleList.IndexOf(item);
            for (int i = 0; i < cycleList.Count; i++)
            {
                string prefix = i == index || i == cycleList.Count - 1
                                    ? "#" + Constants.Tab.Substring(1)
                                    : Constants.Tab;
                builder.AppendLine($"{prefix}{cycleList[i]}");
            }

            return builder.ToString();
        }

        public void Dispose()
        {
            if (parent == null)
            {
                cycleList.Clear();
                resetCycleCheckerInstanceAction?.Invoke();
            }
        }

        public bool HasItem(T item)
        {
            return parent?.HasItem(item) ?? cycleList.Contains(item);
        }
    }
}
