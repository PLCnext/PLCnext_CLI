#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Deploy;

internal class AppDeployEngine : IDeployService
{
    private readonly IBinariesLocator binariesLocator;
    private readonly IFileSystem fileSystem;
    private readonly IProcessManager processManager;
    private readonly IEnvironmentService environmentService;
    private readonly ExecutionContext executionContext;

    public AppDeployEngine(IBinariesLocator binariesLocator, IFileSystem fileSystem, IProcessManager processManager, IEnvironmentService environmentService, ExecutionContext executionContext)
    {
        this.binariesLocator = binariesLocator;
        this.fileSystem = fileSystem;
        this.processManager = processManager;
        this.environmentService = environmentService;
        this.executionContext = executionContext;
    }

    private void DeployLinux(string command, string output, VirtualDirectory content)
    {
        if (string.IsNullOrEmpty(command))
        {
            throw new FormattableException("mksquashfs cannot be found. Please install mksquashfs. Try 'apt install squashfs-tools'.");
        }
        
        using IProcess process = processManager.StartProcess(command,
                                                       $"\"{content.FullName}\" \"{output}\" -force-uid 1001 -force-gid 1002",
                                                       executionContext);
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new FormattableException("Deploying the app failed!");
        }
    }

    private void DeployWindows(string command, string output, VirtualDirectory content)
    {
        if (string.IsNullOrEmpty(command) && !HasWslSupport())
        {
            throw new FormattableException("mksquashfs cannot be found. Please install the Windows Linux Subsystem. Try 'wsl --install -d Ubuntu'.");
        }
        
        using IProcess process = string.IsNullOrEmpty(command) 
                                     ?processManager.StartProcess("wsl",
                                                                  $"mksquashfs \"{WslPath(content.FullName)}\" \"{WslPath(output)}\" -force-uid 1001 -force-gid 1002",
                                                                  executionContext)
                                     :processManager.StartProcess(command,
                                                             $"\"{content.FullName}\" \"{output}\" -force-uid 1001 -force-gid 1002",
                                                             executionContext);
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new FormattableException("Deploying the app failed!");
        }
        
        bool HasWslSupport()
        {
            using IProcess searchProcess = processManager.StartProcess("wsl", "mksquashfs -version", 
                                                                       new StringBuilderUserInterface(executionContext));
            searchProcess.WaitForExit();
            return searchProcess.ExitCode == 0;
        }

        string WslPath(string path)
        {
            return Regex.Replace(path, @"^(?<drive>\w+):\\",
                                 m => $"/mnt/{m.Groups["drive"].Value.ToLowerInvariant()}/")
                        .Replace('\\', '/');
        }
    }

    public void DeployFiles(Entity dataModel)
    {
        string command = binariesLocator.GetExecutableCommand("mksquashfs");
        
        Entity project = dataModel.Root;
        CommandEntity commandOrigin = CommandEntity.Decorate(dataModel.Origin);
        VirtualDirectory output = fileSystem.GetDirectory(commandOrigin.Output, project.Path);
        VirtualDirectory content = fileSystem.GetDirectory("content", project.Path);
        string appName = $"{project.Name}.app";
        string outputFile = Path.Combine(output.FullName, appName);
            
        VirtualFile oldFile = fileSystem.GetFile(outputFile);
        if (oldFile.Exists)
        {
            oldFile.Delete();
            executionContext.Observable.OnNext(new Change(() => oldFile.UnDelete(), $"Deleted {oldFile.FullName}"));
        }
        if (environmentService.Platform == OSPlatform.Windows)
        {
            DeployWindows(command, outputFile, content);
        }
        else
        {
            DeployLinux(command, outputFile, content);
        }

        executionContext.WriteInformation($"App was packed into {outputFile}");
    }
}