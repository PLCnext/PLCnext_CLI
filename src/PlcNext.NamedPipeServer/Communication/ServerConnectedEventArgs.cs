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

namespace PlcNext.NamedPipeServer.Communication
{
    public class ServerConnectedEventArgs : EventArgs
    {
        public ServerConnectedEventArgs(ICommunicationProtocol protocol, bool heartbeat)
        {
            Protocol = protocol;
            Heartbeat = heartbeat;
        }

        public ICommunicationProtocol Protocol { get; }
        public bool Heartbeat { get; }
    }
}
