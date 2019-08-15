#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;

namespace PlcNext.Common.Tools.UI
{
    internal class ConsoleUserInterface : IUserInterface
    {
        private enum TraceSeverity
        {
            Verbose,
            Information,
            Warning,
            Error
        }

        private bool showVerbose;
        private bool beQuiet;
        private bool paused;
        private readonly List<(string, TraceSeverity)> storedMessages = new List<(string, TraceSeverity)>();
        private readonly ILog log;

        public ConsoleUserInterface(ISettingsProvider settingsProvider, IEnvironmentService environmentService, IFileSystem fileSystem, ILog log)
        {
            this.log = log;
            try
            {
                string path = ResolvePathNames(settingsProvider.Settings.LogFilePath.CleanPath());
                Uri pathUri = new Uri(path, UriKind.RelativeOrAbsolute);
                if (!pathUri.IsAbsoluteUri)
                {
                    path = Path.Combine(environmentService.AssemblyDirectory, path);
                }
            }
            catch (Exception e)
            {
                WriteError($"Not possible to initialize file trace listener {e}");
            }

            string ResolvePathNames(string unresolved)
            {
                string resolved = unresolved;
                Regex resolvePattern = new Regex(@"{(?<resolvable>\w+)}");
                Match resolveMatch = resolvePattern.Match(unresolved);
                while (resolveMatch.Success)
                {
                    string key = resolveMatch.Groups["resolvable"].Value;
                    if (environmentService.PathNames.ContainsKey(key))
                    {
                        resolved = resolved.Replace(resolveMatch.Value, environmentService.PathNames[key]);
                    }

                    resolveMatch = resolvePattern.Match(resolved);
                }

                return resolved;
            }
        }

        public void WriteInformation(string message)
        {
            PostMessage((message, TraceSeverity.Information));
            log.LogInformation(message);
        }

        public void WriteVerbose(string message)
        {
            if (showVerbose)
            {
                PostMessage((message, TraceSeverity.Verbose));
            }
            log.LogVerbose(message);
        }

        public void WriteError(string message)
        {
            PostMessage((message, TraceSeverity.Error));
            log.LogError(message);
        }

        public void SetVerbosity(bool verbose)
        {
            showVerbose = verbose;
        }

        public void WriteWarning(string message)
        {
            PostMessage((message, TraceSeverity.Warning));
            log.LogWarning(message);
        }

        public void PauseOutput()
        {
            paused = true;
            storedMessages.Clear();
        }

        public void ResumeOutput()
        {
            paused = false;
            foreach ((string message, TraceSeverity severity) stored in storedMessages)
            {
                PostMessage(stored);
            }
        }

        public void SetQuiet(bool quiet)
        {
            beQuiet = quiet;
        }

        private void PostMessage((string message, TraceSeverity severity) message)
        {
            if (beQuiet)
            {
                return;
            }
            if (paused)
            {
                storedMessages.Add(message);
                return;
            }

            switch (message.severity)
            {
                case TraceSeverity.Verbose:
                    Console.WriteLine(message.message);
                    break;
                case TraceSeverity.Information:
                    Console.WriteLine(message.message);
                    break;
                case TraceSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine(message.message);
                    Console.ResetColor();
                    break;
                case TraceSeverity.Warning:
                    Console.Error.WriteLine(message.message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
