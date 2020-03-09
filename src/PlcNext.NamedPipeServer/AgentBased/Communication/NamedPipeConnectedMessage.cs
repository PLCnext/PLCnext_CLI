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
using System.IO.Pipes;
using Agents.Net;

namespace PlcNext.NamedPipeServer.AgentBased.Communication
{
    public sealed class NamedPipeConnectedMessage : Message, IDisposable
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition NamedPipeConnectedMessageDefinition { get; } =
            new MessageDefinition(nameof(NamedPipeConnectedMessage));

        #endregion

        public NamedPipeConnectedMessage(PipeStream readStream, PipeStream writeStream, string address, bool isClient,
                                         Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, NamedPipeConnectedMessageDefinition, childMessages)
        {
            ReadStream = readStream;
            WriteStream = writeStream;
            Address = address;
            IsClient = isClient;
        }

        public NamedPipeConnectedMessage(PipeStream readStream, PipeStream writeStream, string address, bool isClient,
                                         IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, NamedPipeConnectedMessageDefinition, childMessages)
        {
            ReadStream = readStream;
            WriteStream = writeStream;
            Address = address;
            IsClient = isClient;
        }
        public PipeStream ReadStream { get; }
        public PipeStream WriteStream { get; }
        public string Address { get; }
        public bool IsClient { get; }

        protected override string DataToString()
        {
            return $"{nameof(Address)}: {Address}; {nameof(IsClient)}: {IsClient}";
        }

        public void Dispose()
        {
            ReadStream?.Dispose();
            WriteStream?.Dispose();
        }
    }
}
