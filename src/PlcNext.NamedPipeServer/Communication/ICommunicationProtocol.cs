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
using System.Threading;
using System.Threading.Tasks;

namespace PlcNext.NamedPipeServer.Communication
{
    public interface ICommunicationProtocol : IDisposable
    {
        void SendMessage(Stream message, Action messageCompletedAction = null);
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<EventArgs> Error;
        void Start();
        void FlushReceivedMessages();
    }
}