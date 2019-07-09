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
using System.Threading.Tasks;
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.Communication
{
    internal interface IMessageSender : IInstanceMessageSender
    {
        void SendCommandReply(Command command, Action messageCompletedAction = null);
        void SendHandshakeReply(bool success);
        void SendMessage(string messageText, MessageType messageType, Command command);
        void SendProgress(CommandProgress progress);
        void SendHeartbeat(Action messageCompletedAction = null);
    }
}
