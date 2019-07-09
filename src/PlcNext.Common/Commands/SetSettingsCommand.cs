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
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class SetSettingsCommand : SyncCommand<SetSettingsCommandArgs>
    {
        private readonly ISettingsProvider settingsProvider;

        public SetSettingsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ISettingsProvider settingsProvider) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer)
        {
            this.settingsProvider = settingsProvider;
        }

        protected override int Execute(SetSettingsCommandArgs args, ChangeObservable observable)
        {
            using (IEditableSettings editableSettings = settingsProvider.StartSettingTransaction())
            {
                if (args.Add)
                {
                    editableSettings.AddSetting(args.Key, args.Value);
                }
                else if (args.Clear)
                {
                    editableSettings.ClearSetting(args.Key);
                }
                else if (args.Remove)
                {
                    editableSettings.RemoveSetting(args.Key, args.Value);
                }
                else
                {
                    editableSettings.SetSetting(args.Key, args.Value);
                }
            }

            return 0;
        }
    }
}
