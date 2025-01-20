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
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Shouldly;

namespace Test.PlcNext
{
    public static class ScenarioExtensions
    {
        public static async Task RunAsyncWithTimeout(this IScenarioRunner<NoContext> runner, int timeout = 20000)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task runnerTask = runner.RunAsync();
            Task completedTask = await Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token));
            completedTask.ShouldBe(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            taskCancel.Cancel();
            completedTask.Wait(CancellationToken.None);
        } 

        public static void RunWithTimeout(Action runAction, int timeout = 20000, bool checkTimeout = true)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task runnerTask = Task.Run(runAction,taskCancel.Token);
            Task completedTask = Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token)).GetAwaiter().GetResult();
            if (checkTimeout)
            {
                completedTask.ShouldBe(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            }
            taskCancel.Cancel();
            completedTask.Wait(CancellationToken.None);
        } 

        public static T RunWithTimeout<T>(Func<T> runAction, int timeout = 20000)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task<T> runnerTask = Task.Run(runAction,taskCancel.Token);
            Task completedTask = Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token)).GetAwaiter().GetResult();
            completedTask.ShouldBe(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            taskCancel.Cancel();
            completedTask.Wait(CancellationToken.None);
            return ((Task<T>) completedTask).Result;
        } 
    }
}