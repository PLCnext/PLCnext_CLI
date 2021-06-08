#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics;
using PlcNext.Common.Tools.UI;
using Xunit.Abstractions;

namespace Test.PlcNext.SystemTests.Tools
{
    internal class LogTracer : ILog
    {
        private readonly Action<string> printMessage;

        public LogTracer(ITestOutputHelper output) : this(m =>
        {
            output.WriteLine(m);
            Debug.WriteLine(m);
        })
        {
        }

        public LogTracer(Action<string> printMessage)
        {
            this.printMessage = printMessage;
        }

        public void LogVerbose(string message)
        {
            printMessage($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff} - VERBOSE - {message}");
        }

        public void LogInformation(string message)
        {
            printMessage($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff} - INFORMATION - {message}");
        }

        public void LogWarning(string message)
        {
            printMessage($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff} - WARNING - {message}");
        }

        public void LogError(string message)
        {
            printMessage($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff} - ERROR - {message}");
        }
    }
}
