#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Installation.SDK
{
    internal class SdkInstaller : ISdkInstaller
    {
        private readonly IFileUnpackService fileUnpackService;
        private readonly IProgressVisualizer progressVisualizer;
        private readonly ISettingsProvider settingsProvider;

        public SdkInstaller(IFileUnpackService fileUnpackService, IProgressVisualizer progressVisualizer, ISettingsProvider settingsProvider)
        {
            this.fileUnpackService = fileUnpackService;
            this.progressVisualizer = progressVisualizer;
            this.settingsProvider = settingsProvider;
        }

        public async Task InstallSdk(VirtualFile packedSdk, VirtualDirectory destination, ChangeObservable observable,
                                     bool force)
        {
            using (IEditableSettings editableSettings = settingsProvider.StartSettingTransaction())
            {
                if (!force && destination.Files(searchRecursive: true).Any())
                {
                    throw new FileExistsException(destination.FullName);
                }

                if(force)
                {
                    destination.Clear();
                    observable.OnNext(new Change(destination.UnClear, "Cleared destination directory."));
                }

                using (IProgressNotifier progressNotifier = Console.IsInputRedirected || Console.IsOutputRedirected ? null : progressVisualizer.Spawn(1, "Install SDK.", null))
                {
                    await fileUnpackService.Unpack(packedSdk, destination, progressNotifier, observable).ConfigureAwait(false);
                    observable.OnNext(new Change(destination.Clear, "Unpacked to destination directory."));
                }

                editableSettings.AddSetting(Constants.SdkPathsKey, $"{destination.FullName}");
             }
        }
    }
}
