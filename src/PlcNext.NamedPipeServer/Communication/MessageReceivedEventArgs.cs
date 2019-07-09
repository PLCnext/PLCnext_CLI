#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;

namespace PlcNext.NamedPipeServer.Communication
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Stream message)
        {
            Message = message;
        }

        public Stream Message { get; }
    }
}
