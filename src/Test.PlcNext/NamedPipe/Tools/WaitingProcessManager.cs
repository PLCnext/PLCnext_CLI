#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;

namespace Test.PlcNext.NamedPipe.Tools
{
    internal class WaitingProcessManager : IProcessManager
    {
        public WaitingProcessManager(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }

        public IEnumerable<int> GetOtherInstancesProcessIds()
        {
            return Enumerable.Empty<int>();
        }

        public IProcess StartProcess(string fileName, string arguments, IUserInterface userInterface, string workingDirectory = null,
                                     bool showOutput = true, bool showError = true, bool killOnDispose = true)
        {
            return new WaitingProcess(CancellationToken);
        }

        public IProcess StartProcessWithSetup(string fileName, string arguments,
                              IUserInterface userInterface, string setup,
                              string workingDirectory = null,
                              bool showOutput = true, bool showError = true,
                              bool killOnDispose = true)
        {
            return StartProcess(fileName, arguments, userInterface, workingDirectory, showOutput, showError, killOnDispose);
        }

        private class WaitingProcess : IProcess
        {
            private readonly CancellationToken cancellationToken;

            public WaitingProcess(in CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public void Dispose()
            {
                
            }

            public async Task WaitForExitAsync()
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }

            public void WaitForExit()
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
            }

            public int ExitCode { get; } = 0;
        }
    }
}