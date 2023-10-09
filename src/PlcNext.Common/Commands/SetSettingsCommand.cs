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
using Newtonsoft.Json.Linq;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Commands
{
    internal class SetSettingsCommand : SyncCommand<SetSettingsCommandArgs>
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly IEntityFactory entityFactory;

        public SetSettingsCommand(ITransactionFactory transactionFactory, IExceptionHandler exceptionHandler, ExecutionContext executionContext, ICommandResultVisualizer commandResultVisualizer, ISettingsProvider settingsProvider, IEntityFactory entityFactory) : base(transactionFactory, exceptionHandler, executionContext, commandResultVisualizer, true)
        {
            this.settingsProvider = settingsProvider;
            this.entityFactory = entityFactory;
        }

        protected override CommandResult ExecuteDetailed(SetSettingsCommandArgs args, ChangeObservable observable)
        {
            Entity rootEntity = entityFactory.Create(Guid.NewGuid().ToByteString(), args).Root;
            ProjectEntity project = ProjectEntity.Decorate(rootEntity);
            if (project.Version.Major > project.ToolProjectVersion.Major)
            {
                throw new ProjectVersionTooHighException($"{project.ToolProjectVersion}", $"{project.Version}");
            }
            if (args.Description)
            {
                if (string.IsNullOrEmpty(args.Key))
                {
                    var result = new JObject();
                    foreach (var key in settingsProvider.GetSettingKeys())
                    {
                        result.Add(GetSingleDescription(key));
                    }
                    return new CommandResult(0, new JObject(result));
                }
                return new CommandResult(0,  new JObject(GetSingleDescription(args.Key)));
            }
            if (string.IsNullOrEmpty(args.Key))
            {
                throw new SettingKeyIsEmptyException(string.Join(", ", settingsProvider.GetSettingKeys()));
            }
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

            return new CommandResult(0, null);
            
            object GetSingleDescription(string key)
            {
                return new JProperty(key, settingsProvider.GetSettingDescription(key));
            }
        }
    }
}
