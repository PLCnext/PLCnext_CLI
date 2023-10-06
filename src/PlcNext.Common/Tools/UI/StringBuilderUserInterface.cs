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
using System.Text;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Tools.UI
{
    internal class StringBuilderUserInterface : IUserInterface
    {
        private readonly Regex cleanRegex = new Regex(@"^\[.*\]:");
        private readonly StringBuilder informationBuilder = new StringBuilder();
        private readonly StringBuilder verboseBuilder = new StringBuilder();
        private readonly StringBuilder warningBuilder = new StringBuilder();
        private readonly StringBuilder errorBuilder = new StringBuilder();
        private readonly bool writeVerbose;
        private readonly bool writeInformation;
        private readonly bool writeWarning;
        private readonly bool writeError;
        private readonly ILog log;

        public string Verbose => verboseBuilder.ToString();
        public string Information => informationBuilder.ToString();
        public string Warning => warningBuilder.ToString();
        public string Error => errorBuilder.ToString();

        private string prefix = string.Empty;

        public StringBuilderUserInterface(ILog log, bool writeVerbose = false, bool writeInformation = false,
                                          bool writeWarning = false, bool writeError = false)
        {
            this.log = log;
            this.writeVerbose = writeVerbose;
            this.writeInformation = writeInformation;
            this.writeWarning = writeWarning;
            this.writeError = writeError;
        }

        public void WriteInformation(string message, bool withNewLine = true)
        {
            if (writeInformation)
            {
                string suffix = withNewLine ? Environment.NewLine : string.Empty;
                informationBuilder.Append(prefix + Clean(message) + suffix);
                log.LogInformation(prefix + message);
            }
        }

        public string Clean(string message)
        {
            if (cleanRegex.IsMatch(message))
            {
                message = message.Substring(message.IndexOf(':', StringComparison.Ordinal) + 1).TrimStart();
            }

            return message;
        }

        public void WriteVerbose(string message, bool withNewLine = true)
        {
            if (writeVerbose)
            {
                string suffix = withNewLine ? Environment.NewLine : string.Empty;
                verboseBuilder.Append(prefix + Clean(message) + suffix);
                log.LogVerbose(prefix + message);
            }
        }

        public void WriteError(string message, bool withNewLine = true)
        {
            if (writeError)
            {
                string suffix = withNewLine ? Environment.NewLine : string.Empty;
                errorBuilder.Append(prefix + Clean(message) + suffix);
                log.LogError(prefix + message);
            }
        }

        public void SetVerbosity(bool verbose)
        {
            //do nothing
        }

        public void WriteWarning(string message, bool withNewLine = true)
        {
            if (writeWarning)
            {
                string suffix = withNewLine ? Environment.NewLine : string.Empty;
                warningBuilder.Append(prefix + Clean(message) + suffix);
                log.LogWarning(prefix + message);
            }
        }

        public void PauseOutput()
        {
            //do nothing
        }

        public void ResumeOutput()
        {
            //do nothing
        }

        public void SetQuiet(bool quiet)
        {
            //do nothing
        }

        public void SetPrefix(string prefix)
        {
            this.prefix = prefix;
        }
    }
}
