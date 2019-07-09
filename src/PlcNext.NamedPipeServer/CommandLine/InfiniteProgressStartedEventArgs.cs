#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal class InfiniteProgressStartedEventArgs : EventArgs
    {
        public InfiniteProgressStartedEventArgs(string message, Command command)
        {
            Message = message;
            Command = command;
        }

        public string Message { get; }
        
        public Command Command { get; }
        
    }
}