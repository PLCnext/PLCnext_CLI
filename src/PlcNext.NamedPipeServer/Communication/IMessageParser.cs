#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.NamedPipeServer.Communication
{
    internal interface IMessageParser
    {
        event EventHandler<CommandEventArgs> CommandIssued;
        event EventHandler<CommandEventArgs> CommandCanceled;
        event EventHandler<EventArgs> SuicideIssued;
        event EventHandler<HandshakeEventArgs> HandshakeCompleted;
    }
}
