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
        private readonly List<(string, TraceSeverity, bool)> storedMessages = new List<(string, TraceSeverity, bool)>();
        private readonly ILog log;
        private string prefix = string.Empty;

        private const int MaxCharsPerLine = 80;
        private int currentLineCharCount = 0;
        private int currentErrorLineCharCount = 0;
        private bool wasLastWriteWithNewLine = false;
        private bool wasLastErrorWriteWithNewLine = false;

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

        public ConsoleUserInterface(ISettingsProvider settingsProvider, IEnvironmentService environmentService, ILog log)
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

        public void WriteInformation(string message, bool withNewLine = true)
        {
            PostMessage((message, TraceSeverity.Information, withNewLine));
            log.LogInformation(message);
        }

        public void WriteVerbose(string message, bool withNewLine = true)
        {
            if (showVerbose)
            {
                PostMessage((message, TraceSeverity.Verbose, withNewLine));
            }
            log.LogVerbose(message);
        }

        public void WriteError(string message, bool withNewLine = true)
        {
            PostMessage((message, TraceSeverity.Error, withNewLine));
            log.LogError(message);
        }

        public void SetVerbosity(bool verbose)
        {
            showVerbose = verbose;
        }

        public void WriteWarning(string message, bool withNewLine = true)
        {
            PostMessage((message, TraceSeverity.Warning, withNewLine));
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
            foreach ((string message, TraceSeverity severity, bool withNewLine) stored in storedMessages)
            {
                PostMessage(stored);
            }
        }

        public void SetQuiet(bool quiet)
        {
            beQuiet = quiet;
        }

        public void SetPrefix(string prefix)
        {
            this.prefix = prefix;
        }

        private void PostMessage((string message, TraceSeverity severity, bool withNewLine) message)
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

            string resultPrefix = ResultPrefix(message.withNewLine, message.severity is TraceSeverity.Error or TraceSeverity.Warning);

            string suffix = message.withNewLine ? Environment.NewLine : string.Empty;
            string resultMessage = resultPrefix + message.message + suffix;

            switch (message.severity)
            {
                case TraceSeverity.Verbose:
                case TraceSeverity.Information:
                    WrappedConsoleWrite(message.withNewLine, resultMessage);
                    break;
                case TraceSeverity.Error:
                    SwitchColorsToError();
                    WrappedConsoleErrorWrite(message.withNewLine, resultMessage);
                    Console.ResetColor();
                    break;
                case TraceSeverity.Warning:
                    SwitchColorsToWarning();
                    WrappedConsoleErrorWrite(message.withNewLine, resultMessage);
                    Console.ResetColor();
                    break;
                default:
                    throw new InvalidOperationException();
            }
            
            void WrappedConsoleWrite(bool withNewLine, string s)
            {
                if (!withNewLine)
                {
                    if (currentLineCharCount + s.Length >= MaxCharsPerLine)
                    {
                        int charsFittingOnLine = MaxCharsPerLine - currentLineCharCount;
                        Console.Write(s.Substring(0, charsFittingOnLine) + Environment.NewLine);

                        s = prefix + s.Substring(charsFittingOnLine,
                            s.Length - charsFittingOnLine);

                        currentLineCharCount = s.Length;
                    }
                    else
                    {
                        currentLineCharCount += s.Length;
                    }
                }

                Console.Write(s);

                if (withNewLine)
                {
                    currentLineCharCount = 0;
                }

                wasLastWriteWithNewLine = withNewLine;
            }
            
            void WrappedConsoleErrorWrite(bool withNewLine, string s)
            {
                if (!withNewLine)
                {
                    if (currentErrorLineCharCount + s.Length >= MaxCharsPerLine)
                    {
                        int charsFittingOnLine = MaxCharsPerLine - currentErrorLineCharCount;
                        Console.Error.Write(s.Substring(0, charsFittingOnLine) + Environment.NewLine);

                        s = prefix + s.Substring(charsFittingOnLine,
                            s.Length - charsFittingOnLine);
                    }
                    currentErrorLineCharCount = s.Length;
                }
                else
                {
                    currentErrorLineCharCount += s.Length;
                }
                
                Console.Error.Write(s);
                

                if (withNewLine)
                {
                    currentErrorLineCharCount = 0;
                }
                
                wasLastErrorWriteWithNewLine = withNewLine;
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

        /// <summary>
        /// Computes the real prefix necessary to have progress dots <see cref="ConsoleInfiniteProgressNotifier"/>
        /// and process output on separated lines with correct prefixes.
        /// </summary>
        /// <param name="hasMessageNewLine">Whether the current message will end with a newline char.</param>
        /// <param name="isErrorConsole">Whether the output will be on the Console.Error Textwriter.</param>
        /// <returns>The correct prefix.</returns>
        private string ResultPrefix(bool hasMessageNewLine, bool isErrorConsole = false)
        {
            string result;
            // new lines should always start with the prefix
            if (isErrorConsole?wasLastErrorWriteWithNewLine:wasLastWriteWithNewLine)
            {
                result = prefix;
            }
            // when the last line was without newLine at the end it was a progress dot
            else
            {
                // this message is no progress dot and should start on a new line with prefix
                if (hasMessageNewLine)
                {
                    result = Environment.NewLine + prefix;
                }
                // progress dots shall follow each other directly
                else
                {
                    result = string.Empty;
                }
            }

            return result;
        }
    }
}
