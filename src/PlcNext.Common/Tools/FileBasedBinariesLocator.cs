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
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools
{
    internal class FileBasedBinariesLocator : IBinariesLocator
    {
        private readonly IEnvironmentService environmentService;
        private readonly IFileSystem fileSystem;
        private readonly IUserInterface userInterface;
        private readonly IProcessManager processManager;
        private readonly ISettingsProvider settingsProvider;
        private readonly Dictionary<string, BinaryLocator> locators = new Dictionary<string, BinaryLocator>();

        public FileBasedBinariesLocator(IEnvironmentService environmentService, IFileSystem fileSystem, IUserInterface userInterface, IProcessManager processManager, ISettingsProvider settingsProvider)
        {
            this.environmentService = environmentService;
            this.fileSystem = fileSystem;
            this.userInterface = userInterface;
            this.processManager = processManager;
            this.settingsProvider = settingsProvider;
        }

        public VirtualFile GetExecutable(string executableKey, string baseDirectory = "")
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                baseDirectory = environmentService.AssemblyDirectory;
            }

            BinaryLocator locator = GetLocator(baseDirectory);
            return locator.Contains(executableKey) ? locator[executableKey] : null;
        }

        public string GetExecutableCommand(string executableKey, string baseDirectory = "")
        {
            bool systemExecutableExists = SystemExecutableExists();
            VirtualFile localExecutable = GetExecutable(executableKey, baseDirectory);
            if (settingsProvider.Settings.UseSystemCommands)
            {
                return systemExecutableExists ? executableKey : localExecutable?.FullName;
            }

            return localExecutable?.FullName ?? (systemExecutableExists ? executableKey : null);

            bool SystemExecutableExists()
            {
                string command = "which";
                if (environmentService.Platform == OSPlatform.Windows)
                {
                    command = "where";
                }

                using (IProcess process = processManager.StartProcess(command, executableKey, userInterface,
                                                                      showOutput: false, showError: false))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
        }

        private BinaryLocator GetLocator(string baseDirectory)
        {
            if (locators.TryGetValue(baseDirectory, out BinaryLocator locator)) return locator;
            if (fileSystem.FileExists(Path.Combine(baseDirectory, Constants.FileNamesFile)))
            {
                VirtualFile fileNamesFile = fileSystem.GetFile(Path.Combine(baseDirectory, Constants.FileNamesFile));
                using (Stream stream = fileNamesFile.OpenRead())
                {
                    XDocument document = XDocument.Load(stream);
                    Dictionary<string, VirtualFile> executables = new Dictionary<string, VirtualFile>();
                    foreach (XElement element in document.Element("names")?.Elements() ??
                                                 Enumerable.Empty<XElement>())
                    {
                        string name = element.Name.LocalName;
                        string partPath = element.Attribute("path")?.Value ?? string.Empty;
                        string path = Path.Combine(baseDirectory,
                                                   partPath.Replace("/", $"{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
                        if (environmentService.Platform == OSPlatform.Windows)
                        {
                            path = path + ".exe";
                        }

                        if (fileSystem.FileExists(path))
                        {
                            executables.Add(name, fileSystem.GetFile(path));
                        }
                        else
                        {
                            userInterface.WriteVerbose($"While locating binaries the following file was not found {path}.");
                        }
                    }

                    locators.Add(baseDirectory, new BinaryLocator(executables));
                }
            }
            else
            {
                locators.Add(baseDirectory, new BinaryLocator(new Dictionary<string, VirtualFile>()));
            }

            return locators[baseDirectory];
        }

        private class BinaryLocator
        {
            private readonly Dictionary<string, VirtualFile> executables;

            public BinaryLocator(Dictionary<string, VirtualFile> executables)
            {
                this.executables = executables;
            }

            public VirtualFile this[string key] => executables[key];

            public bool Contains(string key)
            {
                return executables.ContainsKey(key);
            }
        }
    }
}
