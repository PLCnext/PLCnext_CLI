#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;

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
        using IProcess process = processManager.StartProcess(command,
                                                             $"-D \"{content.FullName}\" \"{output}\" -u 1001 -g 1002",
                                                             executionContext);
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new FormattableException("Deploying the app failed!");
        }
    }

    public void DeployFiles(Entity dataModel)
    {
        string command = binariesLocator.GetExecutableCommand("mksquashfs");
        if (string.IsNullOrEmpty(command))
        {
            throw new FormattableException("mksquashfs cannot be found. Please install mksquashfs. Try 'apt install squashfs-tools'.");
        }
        
        Entity project = dataModel.Root;
        CommandEntity commandOrigin = CommandEntity.Decorate(dataModel.Origin);
        VirtualDirectory output = fileSystem.GetDirectory(commandOrigin.Output, project.Path);
        VirtualDirectory content = fileSystem.GetDirectory("content", project.Path);
        string appName = $"{project.Name}.app";
        string outputFile = Path.Combine(output.FullName, appName);

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