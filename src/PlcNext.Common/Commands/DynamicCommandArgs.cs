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
using PlcNext.Common.Tools.DynamicCommands;

namespace PlcNext.Common.Commands
{
    public class DynamicCommandArgs : CommandArgs
    {
        public DynamicCommandArgs(CommandDefinition definition)
        {
            Definition = definition;
        }

        public CommandDefinition Definition { get; }
    }
}
