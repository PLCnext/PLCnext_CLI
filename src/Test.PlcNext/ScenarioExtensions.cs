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
using FluentAssertions;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using Nito.AsyncEx.Synchronous;

namespace Test.PlcNext
{
    public static class ScenarioExtensions
    {
        public static async Task RunAsyncWithTimeout(this IScenarioRunner<NoContext> runner, int timeout = 10000)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task runnerTask = runner.RunAsync();
            Task completedTask = await Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token));
            completedTask.Should().Be(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            taskCancel.Cancel();
            completedTask.WaitAndUnwrapException(CancellationToken.None);
        } 

        public static void RunWithTimeout(Action runAction, int timeout = 10000)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task runnerTask = Task.Run(runAction,taskCancel.Token);
            Task completedTask = Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token)).GetAwaiter().GetResult();
            completedTask.Should().Be(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            taskCancel.Cancel();
            completedTask.WaitAndUnwrapException(CancellationToken.None);
        } 

        public static T RunWithTimeout<T>(Func<T> runAction, int timeout = 10000)
        {
            CancellationTokenSource taskCancel = new CancellationTokenSource();
            Task<T> runnerTask = Task.Run(runAction,taskCancel.Token);
            Task completedTask = Task.WhenAny(runnerTask, Task.Delay(timeout, taskCancel.Token)).GetAwaiter().GetResult();
            completedTask.Should().Be(runnerTask, $"test should have finished in {(double) timeout / 1000:F}s");
            taskCancel.Cancel();
            completedTask.WaitAndUnwrapException(CancellationToken.None);
            return ((Task<T>) completedTask).Result;
        } 
    }
}