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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.SDK
{
    internal class SettingsBasedSdkRepository : ISdkRepository
    {
        private readonly Dictionary<Target, SdkInformation> sdks = new Dictionary<Target, SdkInformation>();
        private readonly ISettingsProvider settingsProvider;
        private readonly ISdkExplorer sdkExplorer;
        private readonly ISdkContainer sdkContainer;
        private bool loaded;

        public SettingsBasedSdkRepository(ISettingsProvider settingsProvider, ISdkExplorer sdkExplorer, ISdkContainer sdkContainer)
        {
            this.settingsProvider = settingsProvider;
            this.sdkExplorer = sdkExplorer;
            this.sdkContainer = sdkContainer;
        }

        private Dictionary<Target, SdkInformation> Sdks
        {
            get
            {
                EnsureSdks();
                return sdks;

                void EnsureSdks()
                {
                    if (loaded) return;
                    
                    foreach (string path in settingsProvider.Settings.SdkPaths)
                    {
                        if (!sdkContainer.Contains(path))
                        {
                            Task<bool> added = AddNewSdk(path, true, false);
                            added.Wait();
                            if (!added.Result)
                            {
                                continue;
                            }
                        }
                        SdkInformation sdkInformation = sdkContainer.Get(path);
                        foreach (Target target in sdkInformation.Targets)
                        {
                            if (sdks.ContainsKey(target))
                            {
                                throw new TargetInstalledTwiceException(target.GetLongFullName());
                            }

                            sdks.Add(target, sdkInformation);
                        }
                    }

                    loaded = true;
                    OnLoaded();
                }
            }
        }

        IEnumerable<SdkInformation> ISdkRepository.Sdks => Sdks.Values.Distinct().ToArray();

        public SdkInformation GetSdk(Target target)
        {
            if (!Sdks.TryGetValue(target, out SdkInformation sdkInformation))
            {
                throw new NoSdkForTargetException(target);
            }

            return sdkInformation;
        }

        public IEnumerable<Target> GetAllTargets()
        {
            return Sdks.Keys;
        }

        public IEnumerable<string> SdkPaths => Sdks.Values.Select(s => s.Root.FullName);
        
        public async Task Update(string sdkPath, bool forced = false)
        {
            if (!forced && sdkContainer.Contains(sdkPath))
            {
                return;
            }

            if (sdkContainer.Contains(sdkPath))
            {
                sdkContainer.Remove(sdkPath);
            }

            await AddNewSdk(sdkPath, forced).ConfigureAwait(false);

            OnUpdated();
        }

        private string exploringSdk = string.Empty;

        private async Task<bool> AddNewSdk(string sdkPath, bool forced, bool checkForDuplicate = true)
        {
            if (exploringSdk == sdkPath)
            {
                return false;
            }

            try
            {
                exploringSdk = sdkPath;
                SdkSchema sdkSchema = await sdkExplorer.ExploreSdk(sdkPath, forced).ConfigureAwait(false);
                if (sdkSchema != null)
                {
                    if (checkForDuplicate)
                    {
                        IEnumerable<Target> targets = GetAllTargets();
                        foreach (TargetSchema targetSchema in sdkSchema.Target)
                        {
                            Target target = new Target(targetSchema.name, targetSchema.version);
                            if (targets.Contains(target))
                            {
                                throw new TargetAlreadyInstalledException(target.GetLongFullName());
                            }
                        }
                    }
                    sdkContainer.Add(sdkPath, sdkSchema);
                }
            }
            finally
            {
                exploringSdk = string.Empty;
            }

            return true;
        }

        public void Remove(string sdkPath)
        {
            sdkContainer.Remove(sdkPath);
        }

        public event EventHandler<EventArgs> Loaded;
        public event EventHandler<EventArgs> Updated;

        protected virtual void OnLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
