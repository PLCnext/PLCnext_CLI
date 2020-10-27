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
using System.IO;
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
            settingsObserver.SettingRemoving += OnSettingRemoving;
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

        private void ComparePaths(IReadOnlyCollection<string> newPaths)
        {
            string[] cleanedPaths = newPaths.Select(p => Path.GetFullPath(p.CleanPath())).ToArray();
            
            foreach (string path in cleanedPaths)
            {
                sdkRepository.Update(Path.GetFullPath(path.CleanPath())).ConfigureAwait(false).GetAwaiter().GetResult();
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
                sdkRepository.Remove(Path.GetFullPath(sdkPath.CleanPath()));
            }
        }

        private void OnSettingRemoving(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }

            List<string> removedPaths = new List<string>();
            foreach (string sdkPath in e.SplitValue)
            {
                string path = Path.GetFullPath(sdkPath.CleanPath());
                sdkRepository.Remove(path);
                removedPaths.Add(path);
            }

            e.SplitValue = removedPaths;
        }

        private void OnSettingAdding(object sender, SettingsObserverEventArgs e)
        {
            if (!IsRelevant(e))
            {
                return;
            }
            
            List<string> addedPaths = new List<string>();
            foreach (string sdkPath in e.SplitValue)
            {
                string path = Path.GetFullPath(sdkPath.CleanPath());
                sdkRepository.Update(path, true).ConfigureAwait(false).GetAwaiter().GetResult();
                addedPaths.Add(path);
            }

            e.SplitValue = addedPaths;
        }

        private static bool IsRelevant(SettingsObserverEventArgs settingsObserverEventArgs)
        {
            return settingsObserverEventArgs.Key == Constants.SdkPathsKey;
        }

        public void Dispose()
        {
            settingsObserver.SettingAdding -= OnSettingAdding;
            settingsObserver.SettingRemoving -= OnSettingRemoving;
            settingsObserver.SettingCleared -= OnSettingCleared;
            settingsObserver.SettingSetting -= OnSettingSetting;
            sdkRepository.Loaded -= OnSdksPropertiesLoaded;
        }
    }
}
