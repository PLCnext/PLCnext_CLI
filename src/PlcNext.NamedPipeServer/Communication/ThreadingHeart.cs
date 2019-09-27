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

        private readonly object heartSyncRoot = new object();
        private readonly object liveCounterSyncRoot = new object();

        private CancellationTokenSource heartKillToken;
        private int lifeCounter;
        private bool isAlive;

        private CancellationToken CancellationToken => heartKillToken.Token;

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
                    isAlive = true;
                    StartHeartbeat();
                }
                lifeCounter++;
            }
        }

        public void StopHeartbeat()
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
                    KillHeart();
                }
            }
        }

        public void Dispose()
        {
            KillHeart();
        }

        private void StartHeartbeat()
        {
            HighResolutionTimer timer = null;
            DateTime start = DateTime.Now;
            lock (heartSyncRoot)
            {
                if (!isAlive)
                {
                    return;
                }
                
                heartKillToken = new CancellationTokenSource();
                CancellationToken.Register(StopHeart);
            
                StartTimer(10);
            }

            void StartTimer(int delay)
            {
                timer = new HighResolutionTimer(delay)
                    {UseHighPriorityThread = false};
                timer.Elapsed += TimerOnElapsed;
                timer.Start();
                log.LogVerbose("Heartbeat timer started.");
            }

            void TimerOnElapsed(object sender, HighResolutionTimerElapsedEventArgs e)
            {
                try
                {
                    lock (heartSyncRoot)
                    {
                        if (!isAlive)
                        {
                            return;
                        }

                        timer.Elapsed -= TimerOnElapsed;
                        timer.Stop(false);
                        start = DateTime.Now;
                        log.LogVerbose($"Sending heartbeat message after waiting {e.Delay:F}ms.");
                        messageSender.SendHeartbeat(RestartTimer);
                    }
                }
                catch (Exception)
                {
                    //do nothing as it would crash the application
                }
            }

            void RestartTimer()
            {
                try
                {
                    lock (heartSyncRoot)
                    {
                        if (!isAlive)
                        {
                            return;
                        }
                        
                        DateTime end = DateTime.Now;
                        TimeSpan lastWriteTimeSpan = end >= start ? end - start : TimeSpan.Zero;
                        int delay = Math.Max(
                            CommunicationConstants.HeartbeatInterval - lastWriteTimeSpan.Milliseconds, 0);

                        log.LogVerbose($"Last heartbeat send, wait for {delay}ms.");
                        StartTimer(delay);
                    }
                }
                catch (Exception)
                {
                    //do nothing as it would crash the application
                }
            }

            void StopHeart()
            {
                lock (heartSyncRoot)
                {
                    timer?.Stop(false);
                    heartKillToken?.Dispose();
                    heartKillToken = null;
                }
            }
        }

        private void KillHeart()
        {
            lock (heartSyncRoot)
            {
                isAlive = false;
                heartKillToken?.Cancel();
                heartKillToken?.Dispose();
                heartKillToken = null;
            }
        }
    }
}