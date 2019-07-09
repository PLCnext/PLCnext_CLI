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
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal interface ICommandLineFacade
    {
        Task<Command> ExecuteCommand(Command command);
        void CancelCommand(Command command);
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ProgressReceivedEventArgs> ProgressReceived;
        event EventHandler<InfiniteProgressStartedEventArgs> InfiniteProgressStarted;
        void CancelAllCommands();
    }
}
