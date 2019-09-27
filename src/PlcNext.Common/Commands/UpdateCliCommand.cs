#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.Installation;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class UpdateCliCommand : AsyncCommand<UpdateCliCommandArgs>
    {
        private readonly ICliUpdater cliUpdater;
        private readonly IFileSystem fileSystem;
        private readonly IUserInterface userInterface;
        private readonly IProgressVisualizer progressVisualizer;

        public UpdateCliCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ICliUpdater cliUpdater, IFileSystem fileSystem, IProgressVisualizer progressVisualizer, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.cliUpdater = cliUpdater;
            this.fileSystem = fileSystem;
            this.progressVisualizer = progressVisualizer;
            this.userInterface = userInterface;
        }

        protected override async Task<int> ExecuteAsync(UpdateCliCommandArgs args, ChangeObservable observable)
        {
            string startMessage = string.IsNullOrEmpty(args.File)
                                      ? (args.Version != null
                                             ? $"Updating to version {args.Version}"
                                             : "Updating to newest version")
                                      : $"Updating using the file {args.File}";
            using (IProgressNotifier progressNotifier = progressVisualizer.Spawn(2, startMessage, "Completed update"))
            {
                VirtualFile download;
                VirtualDirectory directory = fileSystem.GetTemporaryDirectory();
                observable.OnNext(new Change(() => {}, $"Create temporary folder {directory.FullName}"));
                if (string.IsNullOrEmpty(args.File))
                {
                    if (await cliUpdater.IsCurrentVersion(args.Version, args.Proxy).ConfigureAwait(false))
                    {
                        directory.Delete();
                        userInterface.WriteInformation("Version is up-to-date.");
                        return 0;
                    }
                    
                    download = await cliUpdater.DownloadVersion(args.Version, directory, progressNotifier, args.Proxy).ConfigureAwait(false);
                }
                else
                {
                    progressNotifier.TickIncrement();
                    if (!fileSystem.FileExists(args.File))
                        throw new FormattableException($"The file {args.File} does not exist.");
                    download = fileSystem.GetFile(args.File);
                }
                
                await cliUpdater.InstallVersion(download, directory, progressNotifier).ConfigureAwait(false);
            }

            return 0;
        }
    }
}
