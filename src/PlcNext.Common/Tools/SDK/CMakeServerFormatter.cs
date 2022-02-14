﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics;
using System.IO;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.SDK
{
    internal class CMakeServerFormatter : IOutputFormatter
    {
        private readonly ILog log;

        public CMakeServerFormatter(ILog log)
        {
            this.log = log;
        }

        public bool CanFormat(FormatterParameters parameters)
        {
            return parameters.TryGet(Constants.MessageFormat, out string messageFormat) &&
                   messageFormat == "cmake-json";
        }

        public IUserInterface Format(IUserInterface wrappedUserInterface)
        {
            return new FormatterInterface(wrappedUserInterface, log);
        }

        private class FormatterInterface : IUserInterface
        {
            private readonly IUserInterface wrappedUserInterface;
            private readonly ILog log;

            public FormatterInterface(IUserInterface wrappedUserInterface, ILog log)
            {
                this.wrappedUserInterface = wrappedUserInterface;
                this.log = log;
            }

            public void WriteInformation(string message, bool withNewLine = true)
            {
                wrappedUserInterface.WriteInformation(message, withNewLine);
            }

            public void WriteVerbose(string message, bool withNewLine = true)
            {
                HandleMessage(message, withNewLine);
            }

            public void WriteError(string message, bool withNewLine = true)
            {
                wrappedUserInterface.WriteError(message, withNewLine);
            }

            public void SetVerbosity(bool verbose)
            {
                wrappedUserInterface.SetVerbosity(verbose);
            }

            public void WriteWarning(string message, bool withNewLine = true)
            {
                wrappedUserInterface.WriteWarning(message, withNewLine);
            }

            public void PauseOutput()
            {
                wrappedUserInterface.PauseOutput();
            }

            public void ResumeOutput()
            {
                wrappedUserInterface.ResumeOutput();
            }

            public void SetQuiet(bool quiet)
            {
                wrappedUserInterface.SetQuiet(quiet);
            }

            public void SetPrefix(string prefix)
            {
                wrappedUserInterface.SetPrefix(prefix);
            }

            private void HandleMessage(string message,bool withNewLine)
            {
                if (!((message.StartsWith("{", StringComparison.Ordinal) &&
                       message.EndsWith("}", StringComparison.Ordinal)) || //For object
                      (message.StartsWith("[", StringComparison.Ordinal) &&
                       message.EndsWith("]", StringComparison.Ordinal)))) //For array
                {
                    return;
                }

                CMakeMessage parsed = CMakeMessage.Parse<CMakeMessage>(message, throwJsonException: false);
                if (parsed is CMakeMessageMessage messageMessage)
                {
                    if (messageMessage.Title.Contains("warning", StringComparison.OrdinalIgnoreCase))
                    {
                        wrappedUserInterface.WriteWarning(messageMessage.Message, withNewLine);
                    }
                    else if(messageMessage.Title.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                            messageMessage.Title.Contains("exception", StringComparison.OrdinalIgnoreCase))
                    {
                        wrappedUserInterface.WriteError(messageMessage.Message, withNewLine);
                    }
                }

                log.LogVerbose($"Received message from cmake server:{Environment.NewLine}" +
                               $"{message}");
            }
        }
    }
}
