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
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.Installation.SDK;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class InstallSdkCommand : AsyncCommand<InstallSdkCommandArgs>
    {
        private readonly ISdkInstaller sdkInstaller;
        private readonly IUserInterface userInterface;
        private readonly IFileSystem fileSystem;

        public InstallSdkCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ISdkInstaller sdkInstaller, IFileSystem fileSystem, IUserInterface userInterface) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.sdkInstaller = sdkInstaller;
            this.fileSystem = fileSystem;
            this.userInterface = userInterface;
        }

        protected override async Task<int> ExecuteAsync(InstallSdkCommandArgs args, ChangeObservable observable)
        {
            VirtualDirectory directory = fileSystem.GetDirectory(args.Destination);

            if (directory.FullName.Contains(' ', StringComparison.Ordinal))
            {
                throw new SdkPathWithSpacesException(directory.FullName);
            }

            VirtualFile sdk;
            if (fileSystem.FileExists(args.Sdk))
            {
                sdk = fileSystem.GetFile(args.Sdk);
            }
            else
            {
                throw new SdkFileNotFoundException(args.Sdk);
            }
            await sdkInstaller.InstallSdk(sdk, directory, observable, args.Force).ConfigureAwait(false);

            userInterface.WriteInformation($"Successfully installed sdk {args.Sdk} in {args.Destination}.");
            return 0;
        }
    }
}
