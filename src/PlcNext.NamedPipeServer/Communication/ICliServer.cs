#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading.Tasks;

namespace PlcNext.NamedPipeServer.Communication
{
    public interface ICliServer
    {
        Task<bool> Start(string serverName, bool heartbeat);
        void Stop();
        event EventHandler<ServerConnectedEventArgs> Connected;
        event EventHandler<EventArgs> Disconnected;
    }
}
