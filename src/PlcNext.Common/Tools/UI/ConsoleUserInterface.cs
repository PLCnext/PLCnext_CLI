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

        private static readonly Dictionary<ConsoleColor, ConsoleColor> ErrorColors = new Dictionary<ConsoleColor, ConsoleColor>
        {
            { ConsoleColor.DarkBlue,ConsoleColor.Red},
            { ConsoleColor.DarkMagenta,ConsoleColor.Red}, //should be DarkCyan but for some reason Powershell reports default DarkBlue as DarkMagenta
            { ConsoleColor.DarkCyan,ConsoleColor.DarkRed},
            { ConsoleColor.DarkGray,ConsoleColor.DarkRed},
            { ConsoleColor.DarkGreen,ConsoleColor.Magenta},
            { ConsoleColor.DarkRed,ConsoleColor.DarkCyan},
            { ConsoleColor.DarkYellow,ConsoleColor.DarkRed},
            { ConsoleColor.Black,ConsoleColor.Red},
            { ConsoleColor.Cyan,ConsoleColor.Red},
            { ConsoleColor.Blue,ConsoleColor.DarkRed},
            { ConsoleColor.Gray,ConsoleColor.Red},
            { ConsoleColor.Green,ConsoleColor.Red},
            { ConsoleColor.Magenta,ConsoleColor.DarkCyan},
            { ConsoleColor.Red,ConsoleColor.DarkMagenta},
            { ConsoleColor.White,ConsoleColor.Red},
            { ConsoleColor.Yellow,ConsoleColor.Red},
        };

        private static readonly Dictionary<ConsoleColor, ConsoleColor> WarningColors = new Dictionary<ConsoleColor, ConsoleColor>
        {
            { ConsoleColor.DarkBlue,ConsoleColor.Yellow},
            { ConsoleColor.DarkMagenta,ConsoleColor.Yellow},
            { ConsoleColor.DarkCyan,ConsoleColor.Yellow},
            { ConsoleColor.DarkGray,ConsoleColor.Yellow},
            { ConsoleColor.DarkGreen,ConsoleColor.Yellow},
            { ConsoleColor.DarkRed,ConsoleColor.Yellow},
            { ConsoleColor.DarkYellow,ConsoleColor.Yellow},
            { ConsoleColor.Black,ConsoleColor.Yellow},
            { ConsoleColor.Cyan,ConsoleColor.Yellow},
            { ConsoleColor.Blue,ConsoleColor.Yellow},
            { ConsoleColor.Gray,ConsoleColor.Yellow},
            { ConsoleColor.Green,ConsoleColor.Yellow},
            { ConsoleColor.Magenta,ConsoleColor.Yellow},
            { ConsoleColor.Red,ConsoleColor.Yellow},
            { ConsoleColor.White,ConsoleColor.DarkYellow},
            { ConsoleColor.Yellow,ConsoleColor.DarkYellow},
        };

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
                    SwitchColorsToError();
                    Console.Error.WriteLine(message.message);
                    Console.ResetColor();
                    break;
                case TraceSeverity.Warning:
                    SwitchColorsToWarning();
                    Console.Error.WriteLine(message.message);
                    Console.ResetColor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            void SwitchColorsToError()
            {
                ConsoleColor foreground = ErrorColors.ContainsKey(Console.BackgroundColor)
                                              ? ErrorColors[Console.BackgroundColor]
                                              : ConsoleColor.Red;
                if (foreground == Console.ForegroundColor)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = foreground;
                }
            }

            void SwitchColorsToWarning()
            {
                ConsoleColor foreground = WarningColors.ContainsKey(Console.BackgroundColor)
                                              ? WarningColors[Console.BackgroundColor]
                                              : ConsoleColor.Red;
                if (foreground == Console.ForegroundColor)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = foreground;
                }
            }
        }
    }
}
