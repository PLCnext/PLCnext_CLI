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
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Settings
{
    internal class FileBasedSettingsProvider : ISettingsProvider, ISettingsObserver
    {
        private readonly IFileSystem fileSystem;
        private readonly IEnvironmentService environmentService;
        private readonly ILog log;
        private string settingsPath;
        private const string SettingsFilePath = "{ApplicationData}/{ApplicationName}/settings.xml";
        private bool loaded;

        private SettingsParser parser;

        public FileBasedSettingsProvider(IFileSystem fileSystem, IEnvironmentService environmentService, ILog log)
        {
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.log = log;
        }

        public Settings Settings => Parser.GetSettings();

        private SettingsParser Parser
        {
            get
            {
                if (parser == null)
                {
                    parser = new SettingsParser(LoadSettings());
                    OnSettingsLoaded();
                    loaded = true;
                }

                return parser;
            }
        }

        private string SettingsPath
        {
            get
            {
                if(settingsPath == null)
                {
                    string path = SettingsFilePath.CleanPath().ResolvePathName(environmentService.PathNames);
                    Uri pathUri = new Uri(path, UriKind.RelativeOrAbsolute);
                    if (!pathUri.IsAbsoluteUri)
                    {
                        path = Path.Combine(environmentService.AssemblyDirectory, path);
                    }
                    settingsPath = path;
                }
                return settingsPath;
            }
        }

        public string GetSetting(string key)
        {
            return Parser.GetSetting(key);
        }

        public IEditableSettings StartSettingTransaction()
        {
            string path = SettingsPath;
            VirtualFile file = fileSystem.GetFile(path);
            if (!file.CheckAccess())
            {
                throw new SettingsReadOnlyException();
            }
            return new EditableSettings(SaveSettings, this);
        }

        private void SetSetting(string key, string value)
        {
            SettingsObserverEventArgs eventArgs = new SettingsObserverEventArgs(key, value);
            OnSettingSetting(eventArgs);
            Parser.SetSetting(key, eventArgs.Value);
            OnSettingSet(eventArgs);
        }

        private void AddSetting(string key, string value)
        {
            SettingsObserverEventArgs eventArgs = new SettingsObserverEventArgs(key, value);
            OnSettingAdding(eventArgs);
            Parser.AddSetting(key, eventArgs.Value);
            OnSettingAdded(eventArgs);
        }

        private void RemoveSetting(string key, string value)
        {
            SettingsObserverEventArgs eventArgs = new SettingsObserverEventArgs(key, value);
            OnSettingRemoving(eventArgs);
            Parser.RemoveSetting(key, eventArgs.Value);
            OnSettingRemoved(eventArgs);
        }

        private void ClearSetting(string key)
        {
            Parser.ClearSetting(key);
            OnSettingCleared(new SettingsObserverEventArgs(key, string.Empty));
        }

        public bool HasSetting(string key)
        {
            return Parser.HasSetting(key);
        }

        public string GetSettingValue(string setting)
        {
            return Parser.GetSetting(setting);
        }

        public IEnumerable<string> GetSettingKeys()
        {
            return Parser.GetSettingsKeys();
        }

        private Settings LoadSettings()
        {
            string path = SettingsPath;
            if (!fileSystem.FileExists(path))
            {
                log.LogInformation("No settings file found. Use default settings:");
                Settings result = new Settings();
                log.LogInformation(JsonConvert.SerializeObject(result, Formatting.Indented));
                return result;
            }
            VirtualFile file = fileSystem.GetFile(path);

            using (Stream stream = file.OpenRead())
            {
                XDocument document = XDocument.Load(stream);
                IEnumerable<string> paths = document.Descendants("SDKS").Any()
                                                ? document.Descendants("SDK").Select(d => d.Value)
                                                : Settings.SdkPathsDefault;
                IEnumerable<string> templates = document.Descendants("Templates").Any()
                                                    ? document.Descendants("Template").Select(d => d.Value)
                                                    : Settings.TemplateLocationsDefault;
                string attributePrefix = document.Descendants("Settings").FirstOrDefault()?
                                                 .Attribute("AttributePrefix")?.Value
                                         ?? Settings.AttributePrefixDefault;
                string cliRpositoryRoot = document.Descendants("Settings").FirstOrDefault()?
                                            .Element("CliRepositoryRoot")?.Value
                                         ?? Settings.CliRepositoryRootDefault;
                string cliRpositoryFileName = document.Descendants("Settings").FirstOrDefault()?
                                             .Element("CliRepositoryFileName")?.Value
                                          ?? Settings.CliRepositoryFileNameDefault;
                string cliRpositorySignatureFileName = document.Descendants("Settings").FirstOrDefault()?
                                                 .Element("CliRepositorySignatureFileName")?.Value
                                              ?? Settings.CliRepositorySignatureFileNameDefault;
                string httpProxy = document.Descendants("Settings").FirstOrDefault()?
                                                          .Element("HttpProxy")?.Value
                                        ?? Settings.HttpProxyDefault;
                string logFilePath = document.Descendants("Settings").FirstOrDefault()?
                                        .Element("LogFilePath")?.Value
                                     ?? Settings.LogFilePathDefault;
                bool systemCommands = bool.TryParse(document.Descendants("Settings").FirstOrDefault()?
                                                       .Element("SystemCommands")?.Value, out bool parsed)
                                          ? parsed
                                          : Settings.UseSystemCommandsDefault;

                log.LogInformation($"Used settings {file.FullName}:");
                Settings result = new Settings(attributePrefix, paths.ToArray(), cliRpositoryRoot, cliRpositoryFileName, cliRpositorySignatureFileName, httpProxy, logFilePath, templates.ToArray(), systemCommands);
                log.LogInformation(JsonConvert.SerializeObject(result, Formatting.Indented));
                return result;
            }
        }

        private void SaveSettings()
        {
            Settings current = Parser.GetSettings();
            string path = SettingsPath;
            VirtualFile file = fileSystem.GetFile(path);
            using (Stream stream = file.OpenWrite())
            {
                stream.SetLength(0);
                XDocument document = new XDocument(new XElement("Settings",
                                                                new XAttribute("AttributePrefix", current.AttributePrefix),
                                                                new XElement("CliRepositoryRoot", current.CliRepositoryRoot),
                                                                new XElement("CliRepositoryFileName", current.CliRepositoryFileName),
                                                                new XElement("CliRepositorySignatureFileName", current.CliRepositorySignatureFileName),
                                                                new XElement("HttpProxy", current.HttpProxy??string.Empty),
                                                                new XElement("LogFilePath", current.LogFilePath ?? "log.txt"),
                                                                new XElement("SystemCommands", current.UseSystemCommands.ToString()),
                                                                new XElement("SDKS",
                                                                             current.SdkPaths.Select(p => new XElement("SDK", p))
                                                                                    .Cast<object>().ToArray()),
                                                                new XElement("Templates",
                                                                             current.TemplateLocations.Select(l => new XElement("Template", l))
                                                                                    .Cast<object>().ToArray())));
                document.Save(stream);
            }
        }

        public event EventHandler<SettingsObserverEventArgs> SettingSetting;
        public event EventHandler<SettingsObserverEventArgs> SettingSet;
        public event EventHandler<SettingsObserverEventArgs> SettingAdding;
        public event EventHandler<SettingsObserverEventArgs> SettingAdded;
        public event EventHandler<SettingsObserverEventArgs> SettingRemoving;
        public event EventHandler<SettingsObserverEventArgs> SettingRemoved;
        public event EventHandler<SettingsObserverEventArgs> SettingCleared;
        private event EventHandler<EventArgs> SettingsLoaded;

        event EventHandler<EventArgs> ISettingsObserver.SettingsLoaded
        {
            add
            {
                SettingsLoaded += value;
                if (loaded)
                {
                    OnSettingsLoaded();
                }
            }
            remove => SettingsLoaded -= value;
        }

        protected virtual void OnSettingSet(SettingsObserverEventArgs e)
        {
            SettingSet?.Invoke(this, e);
        }

        protected virtual void OnSettingAdded(SettingsObserverEventArgs e)
        {
            SettingAdded?.Invoke(this, e);
        }

        protected virtual void OnSettingRemoved(SettingsObserverEventArgs e)
        {
            SettingRemoved?.Invoke(this, e);
        }

        protected virtual void OnSettingCleared(SettingsObserverEventArgs e)
        {
            SettingCleared?.Invoke(this, e);
        }

        protected virtual void OnSettingsLoaded()
        {
            SettingsLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSettingSetting(SettingsObserverEventArgs e)
        {
            SettingSetting?.Invoke(this, e);
        }

        protected virtual void OnSettingAdding(SettingsObserverEventArgs e)
        {
            SettingAdding?.Invoke(this, e);
        }

        protected virtual void OnSettingRemoving(SettingsObserverEventArgs e)
        {
            SettingRemoving?.Invoke(this, e);
        }

        private class EditableSettings : IEditableSettings
        {
            private readonly Action disposeAction;
            private readonly FileBasedSettingsProvider settingsProvider;

            public EditableSettings(Action disposeAction, FileBasedSettingsProvider settingsProvider)
            {
                this.disposeAction = disposeAction;
                this.settingsProvider = settingsProvider;
            }

            public void Dispose()
            {
                disposeAction();
            }

            public void SetSetting(string key, string value)
            {
                settingsProvider.SetSetting(key, value);
            }

            public void AddSetting(string key, string value)
            {
                settingsProvider.AddSetting(key, value);
            }

            public void RemoveSetting(string key, string value)
            {
                settingsProvider.RemoveSetting(key, value);
            }

            public void ClearSetting(string key)
            {
                settingsProvider.ClearSetting(key);
            }
        }
    }
}
