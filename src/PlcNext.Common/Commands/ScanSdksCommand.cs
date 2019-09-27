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
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class ScanSdksCommand : AsyncCommand<ScanSdksCommandArgs>
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly ISdkRepository sdkRepository;

        public ScanSdksCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ISettingsProvider settingsProvider, ISdkRepository sdkRepository) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.settingsProvider = settingsProvider;
            this.sdkRepository = sdkRepository;
        }

        protected override async Task<int> ExecuteAsync(ScanSdksCommandArgs args, ChangeObservable observable)
        {
            foreach (string sdkPath in settingsProvider.Settings.SdkPaths)
            {
                await sdkRepository.Update(sdkPath, true).ConfigureAwait(false);
            }

            return 0;
        }
    }
}
