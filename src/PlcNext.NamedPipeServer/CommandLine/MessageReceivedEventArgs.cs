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
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Command command, string message, MessageType messageType)
        {
            Command = command;
            Message = message;
            MessageType = messageType;
        }

        public Command Command { get; }
        public string Message { get; }
        public MessageType MessageType { get; }
    }
}
