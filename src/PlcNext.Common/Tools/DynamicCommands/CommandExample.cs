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

namespace PlcNext.Common.Tools.DynamicCommands
{
    public class CommandExample
    {
        public CommandExample(string command, string description)
        {
            Command = command;
            Description = description;
        }

        public string Command { get; }
        public string Description { get; }
    }
}
