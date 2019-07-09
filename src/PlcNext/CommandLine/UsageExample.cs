#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.CommandLine
{
    public class UsageExample
    {
        public UsageExample(string helpText, string command)
        {
            HelpText = helpText;
            Command = command;
        }

        public string HelpText { get; }

        public string Command { get; }
    }
}
