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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Installation;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Settings
{
    internal class ProcessBasedSettingsMigrationInstallationStep : IInstallationStep
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly IProcessManager processManager;
        private readonly IBinariesLocator binariesLocator;
        private readonly ExecutionContext executionContext;

        public ProcessBasedSettingsMigrationInstallationStep(ISettingsProvider settingsProvider, IProcessManager processManager, IBinariesLocator binariesLocator, ExecutionContext executionContext)
        {
            this.settingsProvider = settingsProvider;
            this.processManager = processManager;
            this.binariesLocator = binariesLocator;
            this.executionContext = executionContext;
        }

        public async Task Install(VirtualDirectory newVersionDirectory, IProgressNotifier parentProgress)
        {
            executionContext.WriteInformation("Migrating settings");

            VirtualFile executable = GetExecutable();

            IEnumerable<string> availableSettings = (await GetAvailableSettings().ConfigureAwait(false)).ToArray();
            string[] migratableSettings = availableSettings.Where(settingsProvider.HasSetting)
                                                           .ToArray();

            await Migrate().ConfigureAwait(false);

            string[] nonMigratableSettings = settingsProvider.GetSettingKeys()
                                                             .Except(availableSettings)
                                                             .ToArray();

            if (nonMigratableSettings.Any())
            {
                executionContext.WriteInformation("The following settings could not be migrated and are lost:");
                executionContext.WriteInformation(string.Join(", ", nonMigratableSettings));
            }

            async Task Migrate()
            {
                using (IProgressNotifier progress = parentProgress?.Spawn(migratableSettings.Length + 1))
                {
                    foreach (string setting in migratableSettings)
                    {
                        progress?.TickIncrement(message: $"Migrate '{setting}'");
                        string value = settingsProvider.GetSettingValue(setting);
                        if (string.IsNullOrEmpty(value))
                        {
                            await ExecuteCommand($"set setting \"{setting}\" --clear --no-sdk-exploration").ConfigureAwait(false);
                        }
                        else
                        {
                            await ExecuteCommand($"set setting \"{setting}\" \"{settingsProvider.GetSettingValue(setting)}\" --no-sdk-exploration").ConfigureAwait(false);
                        }
                    }
                }
            }

            VirtualFile GetExecutable()
            {
                VirtualFile result = binariesLocator.GetExecutable("application", newVersionDirectory.FullName);

                if (result == null)
                {
                    throw new FormattableException($"No executable found in directory {newVersionDirectory.FullName} to migrate settings.");
                }

                return result;
            }

            async Task<IEnumerable<string>> GetAvailableSettings()
            {
                string command = "get setting --all";
                StringBuilderUserInterface userInterface = await ExecuteCommand(command).ConfigureAwait(false);
                JObject settings = JObject.Parse(userInterface.Information);
                IEnumerable<string> result = settings.Properties().Select(p => p.Name);
                return result;
            }

            async Task<StringBuilderUserInterface> ExecuteCommand(string executableCommand)
            {
                StringBuilderUserInterface ui = new StringBuilderUserInterface(executionContext, writeInformation: true, writeError: true);
                IProcess process = processManager.StartProcess(executable.FullName, executableCommand, ui);
                await process.WaitForExitAsync().ConfigureAwait(false);
                string error = ui.Error;
                if (!string.IsNullOrEmpty(error))
                {
                    throw new SettingsMigrationException(string.Format(CultureInfo.InvariantCulture,
                                                                       ExceptionTexts.SettingsMigrationProcessError,
                                                                       $"{executable.FullName} {executableCommand}", error));
                }

                return ui;
            }
        }
    }
}
