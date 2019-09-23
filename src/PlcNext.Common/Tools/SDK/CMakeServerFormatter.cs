#region Copyright
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

            public void WriteInformation(string message)
            {
                wrappedUserInterface.WriteInformation(message);
            }

            public void WriteVerbose(string message)
            {
                HandleMessage(message);
            }

            public void WriteError(string message)
            {
                wrappedUserInterface.WriteError(message);
            }

            public void SetVerbosity(bool verbose)
            {
                wrappedUserInterface.SetVerbosity(verbose);
            }

            public void WriteWarning(string message)
            {
                wrappedUserInterface.WriteWarning(message);
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

            private void HandleMessage(string message)
            {
                CMakeMessage parsed = CMakeMessage.Parse<CMakeMessage>(message);
                if (parsed is CMakeMessageMessage messageMessage)
                {
                    if (messageMessage.Title.ToLowerInvariant().Contains("warning"))
                    {
                        wrappedUserInterface.WriteWarning(messageMessage.Message);
                    }
                    else if(messageMessage.Title.ToLowerInvariant().Contains("error") ||
                            messageMessage.Title.ToLowerInvariant().Contains("exception"))
                    {
                        wrappedUserInterface.WriteError(messageMessage.Message);
                    }
                }

                log.LogVerbose($"Received message from cmake server:{Environment.NewLine}" +
                               $"{message}");
            }
        }
    }
}
