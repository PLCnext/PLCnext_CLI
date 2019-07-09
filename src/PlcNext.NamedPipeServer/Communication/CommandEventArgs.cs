#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.Communication
{
    internal class CommandEventArgs
    {
        public CommandEventArgs(Command command)
        {
            Command = command;
        }

        public Command Command { get; }
    }
}
