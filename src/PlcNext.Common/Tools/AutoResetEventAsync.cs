#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools
{
    /// <summary>
    ///     Asynchronous version of <see cref="AutoResetEvent" />
    /// </summary>
    public sealed class AutoResetEventAsync
    {
        private static readonly Task<bool> Completed = Task.FromResult(true);

        private readonly ConcurrentQueue<TaskCompletionSource<bool>> handlers =
            new ConcurrentQueue<TaskCompletionSource<bool>>();

        private int isSet;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoResetEventAsync" /> class with a Boolean value indicating whether to set the intial state to signaled.
        /// </summary>
        /// <param name="initialState">true to set the initial state signaled; false to set the initial state to nonsignaled.</param>
        public AutoResetEventAsync(bool initialState)
        {
            this.isSet = initialState ? 1 : 0;
        }

        /// <summary>
        ///     Sets the state of the event to signaled, allowing one waiting continuation to proceed.
        /// </summary>
        public void Set(ILog log = null)
        {
            log?.LogVerbose($"Entered Set");
            if (!this.TrySet())
                return;

            TaskCompletionSource<bool> handler;

            // Notify first alive handler
            while (this.handlers.TryDequeue(out handler))
            {
                log?.LogVerbose($"Dequeue handler");
                if (CheckIfAlive(handler)) // Flag check
                {
                    lock (handler)
                    {
                        if (!CheckIfAlive(handler))
                            continue;

                        if (this.TryReset())
                        {
                            log?.LogVerbose("Setting result for handler.");
                            handler.SetResult(true);
                            log?.LogVerbose("Result for handler set. Handler should return.");
                        }
                        else
                            this.handlers.Enqueue(handler);

                        break;
                    }
                }
            }
            log?.LogVerbose($"Exist Set");
        }

        /// <summary>
        ///     Try to switch the state to signaled from not signaled
        /// </summary>
        /// <returns>
        ///     true if suceeded, false if failed
        /// </returns>
        private bool TrySet()
        {
            return Interlocked.CompareExchange(ref this.isSet, 1, 0) == 0;
        }

        /// <summary>
        ///     Waits for a signal asynchronously
        /// </summary>
        public Task WaitAsync()
        {
            return this.WaitAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Waits for a signal asynchronously
        /// </summary>
        /// <param name="cancellationToken">
        ///     A <see cref="P:System.Threading.Tasks.TaskFactory.CancellationToken" /> to observe while waiting for a signal.
        /// </param>
        /// <exception cref="OperationCanceledException">
        ///     The <paramref name="cancellationToken" /> was canceled.
        /// </exception>
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            // Short path
            if (this.TryReset())
                return Completed;

            cancellationToken.ThrowIfCancellationRequested();

            // Wait for a signal
            var handler = new TaskCompletionSource<bool>(false);

            this.handlers.Enqueue(handler);

            if (CheckIfAlive(handler)) // Flag check
                lock (handler)
                    if (CheckIfAlive(handler) && this.TryReset())
                    {
                        handler.SetResult(true);
                        return handler.Task;
                    }

            cancellationToken.Register(() =>
            {
                if (CheckIfAlive(handler)) // Flag check
                    lock (handler)
                        if (CheckIfAlive(handler))
                            handler.SetCanceled();
            });

            return handler.Task;
        }

        private static bool CheckIfAlive(TaskCompletionSource<bool> handler)
        {
            return handler.Task.Status == TaskStatus.WaitingForActivation;
        }

        private bool TryReset()
        {
            return Interlocked.CompareExchange(ref this.isSet, 0, 1) == 1;
        }
    }
}