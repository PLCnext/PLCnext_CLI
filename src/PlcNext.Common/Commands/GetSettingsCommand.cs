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
            if (args.Description)
            {
                if (args.All || string.IsNullOrEmpty(args.Key))
                {
                    var result = new JObject();
                    foreach (var key in settingsProvider.GetSettingKeys())
                    {
                        result.Add(GetSingleDescription(key));
                    }
                    return new CommandResult(0, new JObject(result));
                }
                return new CommandResult(0, new JObject(GetSingleDescription(args.Key)));
            }
            
            return new CommandResult(0, new SettingCommandResult(args.All
                ? settingsProvider.Settings
                : GetSingleSetting(args.Key)));

            object GetSingleSetting(string key)
            {
                return new JObject(new JProperty(key, settingsProvider.GetSetting(key)));
            }
            
            object GetSingleDescription(string key)
            {
                return new JProperty(key, settingsProvider.GetSettingDescription(key));
            }
        }
    }
}
