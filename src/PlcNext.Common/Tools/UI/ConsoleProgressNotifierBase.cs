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

namespace PlcNext.Common.Tools.UI
{
    internal abstract class ConsoleProgressNotifierBase : IProgressNotifier
    {
        protected readonly IUserInterface UserInterface;
        protected readonly string StartMessage;
        private readonly HashSet<ConsoleProgressNotifierBase> children = new();
        protected readonly IProgressNotifier Parent;
        private const string PrefixArrow = "-> ";
        protected bool wasDisposed = false;

        protected ConsoleProgressNotifierBase(IUserInterface userInterface, string startMessage, IProgressNotifier parent)
        {
            UserInterface = userInterface;
            StartMessage = startMessage;
            Parent = parent;
            SetOutputPrefix();
        }

        protected void SetProgressPrefix()
        {
            if (Parent != null)
            {
                UserInterface.SetPrefix(new string(' ', CalculateDepth()*3) + PrefixArrow);
            }
            else
            {
                UserInterface.SetPrefix(string.Empty);
            }
        }
        
        protected void SetOutputPrefix()
        {
            if (Parent != null)
            {
                UserInterface.SetPrefix(new string(' ', (CalculateDepth()+1)*3) + PrefixArrow);
            }
            else
            {
                UserInterface.SetPrefix(new string(' ', 3) + PrefixArrow);
            }
        }

        private int CalculateDepth()
        {
            int depth = 0;
            IProgressNotifier temp = Parent;
            while (temp is ConsoleProgressNotifierBase tempBase)
            {
                depth++;
                temp = tempBase.Parent;
            }

            return depth;
        }

        public abstract void TickIncrement(double addedProgress = 1, string message = "");

        public abstract void Tick(double totalProgress, string message = "");

        public IProgressNotifier Spawn(double maxTicks, string childStartMessage = "")
        {
            ConsoleProgressNotifier result = new ConsoleProgressNotifier(UserInterface, maxTicks, childStartMessage, parent:this);
            result.Disposed += RemoveChild;
            children.Add(result);
            return result;

            void RemoveChild(object sender, EventArgs eventArgs)
            {
                result.Disposed -= RemoveChild;
                children.Remove(result);
            }
        }

        public IDisposable SpawnInfiniteProgress(string childStartMessage)
        {
            ConsoleInfiniteProgressNotifier result = new ConsoleInfiniteProgressNotifier(UserInterface, childStartMessage, parent:this);
            result.Disposed += RemoveChild;
            children.Add(result);
            return result;

            void RemoveChild(object sender, EventArgs eventArgs)
            {
                result.Disposed -= RemoveChild;
                children.Remove(result);
            }
        }

        public virtual void Dispose()
        {
            if (!wasDisposed)
            {
                foreach (ConsoleProgressNotifierBase child in children.ToArray())
                {
                    child.Dispose();
                }
                OnDisposed();
                SetProgressPrefix();
                wasDisposed = true;
            }
        }

        public event EventHandler<EventArgs> Disposed;

        protected virtual void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
