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
using System.Timers;
using ShellProgressBar;

namespace PlcNext.Common.Tools.UI
{
    internal abstract class ShellProgressNotifierBase : IProgressNotifier
    {
        protected readonly double Scaling;
        private ProgressBarBase progressBar;
        protected string StartMessage;
        protected readonly ProgressBarOptions Options;
        private double currentValue;
        private readonly Timer delayTimer = new Timer(Constants.ProgressBarDelay) { AutoReset = false };
        private readonly HashSet<ShellProgressNotifierBase> children = new HashSet<ShellProgressNotifierBase>();

        protected ShellProgressNotifierBase(double maxTicks, string startMessage, ProgressBarOptions options)
        {
            Scaling = maxTicks < 100 ? 100 / maxTicks : maxTicks > Constants.ProgressMaxResolution ? Constants.ProgressMaxResolution / maxTicks : 1;
            delayTimer.Elapsed += DelayOnElapsed;
            delayTimer.Start();
            StartMessage = startMessage;
            Options = options;
        }

        private void DelayOnElapsed(object sender, ElapsedEventArgs e)
        {
            SpawnProgressBar();
        }

        public ProgressBarBase ProgressBar
        {
            get
            {
                if (progressBar == null)
                {
                    SpawnProgressBar();
                }

                return progressBar;
            }
        }

        private void SpawnProgressBar()
        {
            delayTimer.Stop();
            progressBar = CreateProgressBar();
            progressBar.Tick((int)(currentValue * Scaling));
        }

        protected abstract ProgressBarBase CreateProgressBar();

        public void TickIncrement(double addedProgress = 1, string message = "")
        {
            currentValue += addedProgress;
            UpdateProgressBar(message);
        }

        public void Tick(double totalProgress, string message = "")
        {
            currentValue = totalProgress;
            UpdateProgressBar(message);
        }

        private void UpdateProgressBar(string message)
        {
            if (progressBar == null)
            {
                if (string.IsNullOrEmpty(message))
                {
                    StartMessage = message;
                }
                return;
            }

            int scaledValue = (int)(currentValue * Scaling);
            progressBar.Tick(scaledValue, string.IsNullOrEmpty(message) ? null : message);
        }

        public IProgressNotifier Spawn(double maxTicks, string startMessage = "")
        {
            ShellChildProgressNotifier result = new ShellChildProgressNotifier(maxTicks, startMessage, Options, this);
            result.Disposed += RemoveChild;
            children.Add(result);
            return result;

            void RemoveChild(object sender, EventArgs eventArgs)
            {
                result.Disposed -= RemoveChild;
                children.Remove(result);
            }
        }

        public IDisposable SpawnInfiniteProgress(string startMessage)
        {
            ShellInfinitChildProgressNotifier result = new ShellInfinitChildProgressNotifier(startMessage, Options, this);
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
            foreach (ShellProgressNotifierBase child in children.ToArray())
            {
                child.Dispose();
            }
            delayTimer.Elapsed -= DelayOnElapsed;
            (progressBar as IDisposable)?.Dispose();
            delayTimer.Dispose();
            OnDisposed();
        }

        public event EventHandler<EventArgs> Disposed;

        protected virtual void OnDisposed()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
