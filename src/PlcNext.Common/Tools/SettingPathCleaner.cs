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
using System.Linq;
using System.Text;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;

namespace PlcNext.Common.Tools
{
    internal class SettingPathCleaner : IDisposable
    {
        private readonly ISettingsObserver settingsObserver;

        private static readonly string[] PathSettings =
        {
            nameof(Settings.Settings.SdkPaths),
            nameof(Settings.Settings.TemplateLocations),
            nameof(Settings.Settings.LogFilePath),
        };

        public SettingPathCleaner(ISettingsObserver settingsObserver)
        {
            this.settingsObserver = settingsObserver;
            settingsObserver.SettingAdding += OnSettingChanging;
            settingsObserver.SettingSetting += OnSettingChanging;
            settingsObserver.SettingRemoving += OnSettingChanging;
            settingsObserver.SettingsLoading += OnSettingsLoading;
        }

        private void OnSettingsLoading(object sender, SettingsLoadingEventArgs e)
        {
            e.Settings = new Settings.Settings(e.Settings.AttributePrefix,
                                               e.Settings.SdkPaths.Select(p => p.CleanPath()).ToArray(),
                                               e.Settings.LogFilePath.CleanPath(),
                                               e.Settings.TemplateLocations.Select(p => p.CleanPath()).ToArray(),
                                               e.Settings.UseSystemCommands,
                                               e.Settings.AlwaysWriteExtendedLog,
                                               e.Settings.MSBuildPath);
        }

        private void OnSettingChanging(object sender, SettingsObserverEventArgs e)
        {
            if (PathSettings.Contains(e.Key))
            {
                e.SplitValue = e.SplitValue.Select(p => p.CleanPath()).ToArray();
            }
        }

        public void Dispose()
        {
            settingsObserver.SettingAdding -= OnSettingChanging;
            settingsObserver.SettingSetting -= OnSettingChanging;
            settingsObserver.SettingRemoving -= OnSettingChanging;
            settingsObserver.SettingsLoading -= OnSettingsLoading;
        }
    }
}
