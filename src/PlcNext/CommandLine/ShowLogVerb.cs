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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using PlcNext.Common.Commands;
using PlcNext.Common.Tools;

namespace PlcNext.CommandLine
{
    [Verb("show-log", HelpText = "Opens the location of the log catalog.")]
    public class ShowLogVerb : VerbBase
    {
        protected override Task<int> Execute(ICommandManager commandManager)
        {
            DirectoryInfo directory = new FileInfo(LogHelper.GetLogCatalogLocation()).Directory;
            if (directory != null)
            {
                try
                {
                    if (LifetimeScope.Resolve<IEnvironmentService>().Platform == OSPlatform.Windows)
                    {
                        Process.Start("explorer.exe",directory.FullName);
                    }
                    else
                    {
                        LifetimeScope.Resolve<ExecutionContext>().WriteInformation($"Please open the location {directory.FullName} to access the log.");
                    }
                }
                catch (Exception e)
                {
                    LifetimeScope.Resolve<ExecutionContext>().WriteError($"Error while opening log location {directory.FullName}{Environment.NewLine}" +
                                                                         $"{e}");
                    return Task.FromResult(-1);
                }
            }
            else
            {
                LifetimeScope.Resolve<ExecutionContext>().WriteError($"Could not determine directory of log catalog file {LogHelper.GetLogCatalogLocation()}");
                return Task.FromResult(-1);
            }

            return Task.FromResult(0);
        }
    }
}