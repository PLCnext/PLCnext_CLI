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
using System.Threading.Tasks;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;

namespace PlcNext.Common.Tools.SDK
{
    internal class SdkSettingObserver : IDisposable
    {
        private readonly ISettingsObserver settingsObserver;
        private readonly ISdkRepository sdkRepository;
        private readonly ISettingsProvider settingsProvider;

        public SdkSettingObserver(ISettingsObserver settingsObserver, ISettingsProvider settingsProvider, ISdkRepository sdkRepository)
        {
            this.settingsObserver = settingsObserver;
            this.settingsProvider = settingsProvider;
            this.sdkRepository = sdkRepository;
            settingsObserver.SettingAdding += OnSettingAdding;
            settingsObserver.SettingRemoved += OnSettingRemoved;
            settingsObserver.SettingCleared += OnSettingCleared;
            settingsObserver.SettingSetting += OnSettingSetting;
            sdkRepository.Loaded += OnSdksPropertiesLoaded;
        }

        private void OnSdksPropertiesLoaded(object sender, EventArgs e)
        {
            ComparePaths(settingsProvider.Settings.SdkPaths);
        }

        private void OnSettingSetting(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }

            ComparePaths(e.SplitValue);
        }

        private void ComparePaths(string[] newPaths)
        {
            string[] cleanedPaths = newPaths.Select(p => p.CleanPath()).ToArray();
            
            foreach (string path in cleanedPaths)
            {
                sdkRepository.Update(path.CleanPath()).Wait();
            }

            foreach (string removablePath in sdkRepository.SdkPaths.Except(cleanedPaths).ToArray())
            {
                sdkRepository.Remove(removablePath);
            }
        }

        private void OnSettingCleared(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }

            foreach (string sdkPath in sdkRepository.SdkPaths.ToArray())
            {
                sdkRepository.Remove(sdkPath.CleanPath());
            }
        }

        private void OnSettingRemoved(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }

            foreach (string sdkPath in e.SplitValue)
            {
                sdkRepository.Remove(sdkPath.CleanPath());
            }
        }

        private void OnSettingAdding(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }

            foreach (string path in e.SplitValue)
            {
                sdkRepository.Update(path.CleanPath()).Wait();
            }
        }

        private static bool IsRelevant(SettingsObserverEventArgs settingsObserverEventArgs)
        {
            return settingsObserverEventArgs.Key == Constants.SdkPathsKey;
        }

        public void Dispose()
        {
            settingsObserver.SettingAdding -= OnSettingAdding;
            settingsObserver.SettingRemoved -= OnSettingRemoved;
            settingsObserver.SettingCleared -= OnSettingCleared;
            settingsObserver.SettingSetting -= OnSettingSetting;
            sdkRepository.Loaded -= OnSdksPropertiesLoaded;
        }
    }
}
