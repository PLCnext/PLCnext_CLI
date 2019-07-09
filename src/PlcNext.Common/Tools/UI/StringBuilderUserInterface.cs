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

        public StringBuilderUserInterface(ILog log, bool writeVerbose = false, bool writeInformation = false,
                                          bool writeWarning = false, bool writeError = false)
        {
            this.log = log;
            this.writeVerbose = writeVerbose;
            this.writeInformation = writeInformation;
            this.writeWarning = writeWarning;
            this.writeError = writeError;
        }

        public void WriteInformation(string message)
        {
            if (writeInformation)
            {
                informationBuilder.AppendLine(Clean(message));
                log.LogInformation(message);
            }
        }

        public string Clean(string message)
        {
            if (cleanRegex.IsMatch(message))
            {
                message = message.Substring(message.IndexOf(':') + 1).TrimStart();
            }

            return message;
        }

        public void WriteVerbose(string message)
        {
            if (writeVerbose)
            {
                verboseBuilder.AppendLine(Clean(message));
                log.LogVerbose(message);
            }
        }

        public void WriteError(string message)
        {
            if (writeError)
            {
                errorBuilder.AppendLine(Clean(message));
                log.LogError(message);
            }
        }

        public void SetVerbosity(bool verbose)
        {
            //do nothing
        }

        public void WriteWarning(string message)
        {
            if (writeWarning)
            {
                warningBuilder.AppendLine(Clean(message));
                log.LogWarning(message);
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
    }
}
