#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Tools;

namespace PlcNext.NamedPipeServer.Communication
{
    internal class ThreadingHeart : IHeart, IDisposable
    {
        private readonly IMessageSender messageSender;
        private readonly CommunicationSettings settings;
        private readonly ILog log;

        private readonly object threadSyncRoot = new object();
        private readonly object liveCounterSyncRoot = new object();
        private readonly AsyncAutoResetEvent heartbeatCompleted = new AsyncAutoResetEvent(false);
        
        private Thread heartbeatThread;
        private CancellationTokenSource threadKillToken;
        private int lifeCounter;

        private CancellationToken CancellationToken => threadKillToken.Token;

        public ThreadingHeart(IMessageSender messageSender, CommunicationSettings settings, ILog log)
        {
            this.messageSender = messageSender;
            this.settings = settings;
            this.log = log;
        }

        public void Start()
        {
            if (!settings.HeartbeatEnabled)
            {
                return;
            }

            lock (liveCounterSyncRoot)
            {
                if (lifeCounter == 0)
                {
                    log.LogInformation("Start heartbeat.");
                    CreateThread();
                }
                lifeCounter++;
            }
        }

        public void Stop()
        {
            if (!settings.HeartbeatEnabled)
            {
                return;
            }

            lock (liveCounterSyncRoot)
            {
                lifeCounter = Math.Max(0, lifeCounter - 1);
                if (lifeCounter == 0)
                {
                    log.LogInformation("Stop heartbeat.");
                    KillThread();
                }
            }
        }

        public void Dispose()
        {
            KillThread();
        }

        private async void StartHeartbeat()
        {
            try
            {
                HighResolutionTimer timer = null;
                CancellationToken.Register(() => timer?.Stop(false));
                heartbeatCompleted.Set();
                DateTime start = DateTime.Now - new TimeSpan(0, 0, 0, 0, CommunicationConstants.HeartbeatInterval);
                while (!CancellationToken.IsCancellationRequested)
                {
                    await heartbeatCompleted.WaitAsync(CancellationToken);
                    DateTime end = DateTime.Now;
                    TimeSpan lastWriteTimeSpan = end >= start ? end - start : TimeSpan.Zero;
                    int delay = Math.Max(CommunicationConstants.HeartbeatInterval - lastWriteTimeSpan.Milliseconds, 0);
                    log.LogVerbose($"Last heartbeat send, wait for {delay}ms.");
                    timer = new HighResolutionTimer(delay)
                        {UseHighPriorityThread = false};
                    timer.Elapsed += TimerOnElapsed;
                    timer.Start();
                    
                    void TimerOnElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
                    {
                        timer.Elapsed -= TimerOnElapsed;
                        timer.Stop(false);
                        start = DateTime.Now;
                        messageSender.SendHeartbeat(() =>
                        {
                            heartbeatCompleted.Set();
                        });
                    }
                }
            }
            catch (Exception)
            {
                //Do not log anything as any log will lead to another exception
            }
        }

        private void CreateThread()
        {
            lock (threadSyncRoot)
            {
                threadKillToken = new CancellationTokenSource();
                heartbeatThread = new Thread(StartHeartbeat)
                {
                    Priority = ThreadPriority.Normal,
                    IsBackground = true
                };
                heartbeatThread.Start();
            }
        }

        private void KillThread()
        {
            lock (threadSyncRoot)
            {
                threadKillToken?.Cancel();
                heartbeatThread?.Join(CommunicationConstants.ThreadJoinTimeout);
                threadKillToken?.Dispose();
                threadKillToken = null;
                heartbeatThread = null;
            }
        }
    }
}