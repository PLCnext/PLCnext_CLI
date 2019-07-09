#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Process
{
    internal class ProcessManager : IProcessManager
    {
        private readonly IEnvironmentService environmentService;
        private readonly ExecutionContext executionContext;
        private readonly CancellationToken cancellationToken;

        public ProcessManager(IEnvironmentService environmentService, ExecutionContext executionContext, CancellationToken cancellationToken)
        {
            this.environmentService = environmentService;
            this.cancellationToken = cancellationToken;
            this.executionContext = executionContext;
        }

        public IProcess StartProcess(string fileName, string arguments,
                                     IUserInterface userInterface,
                                     string workingDirectory = null, bool showOutput = true, bool showError = true,
                                     bool killOnDispose = true)
        {
            return new ProcessFacade(fileName, arguments, workingDirectory, executionContext.RedirectOutput(userInterface), showOutput, showError, killOnDispose,
                environmentService.Platform, cancellationToken);
        }

        private static int CurrentProcessId => System.Diagnostics.Process.GetCurrentProcess().Id;
        
        public IEnumerable<int> GetOtherInstancesProcessIds()
        {
            return System.Diagnostics.Process
                         .GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName)
                         .Select(p => p.Id)
                         .Where(id => id != CurrentProcessId)
                         .ToArray();
        }
    }

    internal class ProcessFacade : IProcess
    {
        #region private members
        private readonly System.Diagnostics.Process internalProcess;
        private readonly ExecutionContext executionContext;
        private readonly bool showOutput;
        private readonly bool showError;
        private readonly bool killOnDispose;
        private readonly OSPlatform platform;
        private readonly string processName;
        private readonly bool outputReadStarted;
        private readonly bool errorReadStarted;
        private readonly AsyncAutoResetEvent exitedResetEvent = new AsyncAutoResetEvent(false);
        private bool disposed;
        #endregion

        public ProcessFacade(string fileName, string arguments, string workingDirectory,
                             ExecutionContext executionContext,
                             bool showOutput, bool showError, bool killOnDispose, OSPlatform platform,
                             CancellationToken cancellationToken)
        {
            this.executionContext = executionContext;
            this.showOutput = showOutput;
            this.showError = showError;
            this.killOnDispose = killOnDispose;
            this.platform = platform;
            ProcessStartInfo processInfo = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                processInfo.WorkingDirectory = workingDirectory;
            }
            
            executionContext.WriteVerbose($"Starting process {processInfo.FileName} {processInfo.Arguments} in {processInfo.WorkingDirectory}.");
            internalProcess = System.Diagnostics.Process.Start(processInfo);
            if(internalProcess != null && !internalProcess.HasExited)
            {
                try
                {
                    processName = internalProcess.ProcessName;
                    internalProcess.OutputDataReceived += InternalProcessOnOutputDataReceived;
                    internalProcess.ErrorDataReceived += InternalProcessOnErrorDataReceived;
                    internalProcess.EnableRaisingEvents = true;
                    internalProcess.Exited += InternalProcessOnExited;
                    internalProcess.BeginOutputReadLine();
                    outputReadStarted = true;
                    internalProcess.BeginErrorReadLine();
                    errorReadStarted = true;
                }
                catch (Exception e)
                {
                    processName = "unkown process";
                    executionContext.WriteWarning($"Error while starting process: {e}", false);
                    //this happens when the process exits somewhere in this if clause
                }

                cancellationToken.Register(Dispose);
            }
            else
            {
                processName = "unkown process";
                exitedResetEvent.Set();
            }
        }

        private void InternalProcessOnExited(object sender, EventArgs e)
        {
            
            exitedResetEvent.Set();
        }

        public async Task WaitForExitAsync()
        {
            await exitedResetEvent.WaitAsync();
        }

        public void WaitForExit()
        {
            if (!internalProcess.HasExited)
            {
                internalProcess.WaitForExit();
            }
        }

        private void InternalProcessOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data != null)
            {
                executionContext.WriteInformation($"[{processName}]: {dataReceivedEventArgs.Data}", showOutput);
            }
        }

        private void InternalProcessOnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if(dataReceivedEventArgs.Data != null)
            {
                executionContext.WriteError($"[{processName}]: {dataReceivedEventArgs.Data}", showError);
            }
        }

        public int ExitCode
        {
            get { return internalProcess.ExitCode; }
        }

        private void KillProcessAndChildren(int pid)
        {
            if (platform == OSPlatform.Windows)
            {
                ManagementObjectSearcher processSearcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection processCollection = processSearcher.Get();

                // We must kill child processes first!
                if (processCollection != null)
                {
                    foreach (ManagementObject mo in processCollection)
                    {
                        KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
                    }
                }
            }

            // Then kill parents.
            try
            {
                System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            try
            {
                if (internalProcess != null)
                {
                    internalProcess.OutputDataReceived -= InternalProcessOnOutputDataReceived;
                    internalProcess.ErrorDataReceived -= InternalProcessOnErrorDataReceived;
                    internalProcess.Exited -= InternalProcessOnExited;
                    internalProcess.EnableRaisingEvents = false;
                    if (errorReadStarted)
                    {
                        internalProcess.CancelErrorRead();
                    }
                    if (outputReadStarted)
                    {
                        internalProcess.CancelOutputRead();
                    }
                    if (killOnDispose && !internalProcess.HasExited)
                    {
                        internalProcess.StandardInput.WriteLine();
                        KillProcessAndChildren(internalProcess.Id);
                    }
                }
            }
            catch (Exception e)
            {
                string additionalInformation = string.Empty;
                try
                {
                    if (internalProcess != null)
                    {
                        additionalInformation = $"{internalProcess.MainWindowTitle} - {internalProcess.Id} : {internalProcess.HasExited} - {internalProcess.ExitTime}";
                    }
                }
                catch (Exception)
                {
                    //do nothing only for information gathering
                }
                executionContext.WriteVerbose($"Error while closing process {e}{Environment.NewLine}{additionalInformation}");
            }
        }
    }
}
