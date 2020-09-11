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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Commands.CommandResults;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class GetSettingsCommand : SyncCommand<GetSettingsCommandArgs>
    {
        private readonly ISettingsProvider settingsProvider;

        public GetSettingsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ISettingsProvider settingsProvider) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.settingsProvider = settingsProvider;
        }

        protected override CommandResult ExecuteDetailed(GetSettingsCommandArgs args, ChangeObservable observable)
        {
            return new CommandResult(0, new SettingCommandResult(args.All
                                                                     ? settingsProvider.Settings
                                                                     : GetSingleSetting()));

            object GetSingleSetting()
            {
                return new JObject(new JProperty(args.Key, settingsProvider.GetSetting(args.Key)));
            }
        }
    }
}
