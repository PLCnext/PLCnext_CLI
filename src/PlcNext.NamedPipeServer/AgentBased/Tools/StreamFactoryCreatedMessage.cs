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
using Agents.Net;
using PlcNext.Common.Tools.IO;

namespace PlcNext.NamedPipeServer.AgentBased.Tools
{
    public sealed class StreamFactoryCreatedMessage : Message, IDisposable
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition StreamFactoryCreatedMessageDefinition { get; } =
            new MessageDefinition(nameof(StreamFactoryCreatedMessage));

        #endregion

        public StreamFactoryCreatedMessage(StreamFactory factory, Message predecessorMessage,
                                           params Message[] childMessages)
            : base(predecessorMessage, StreamFactoryCreatedMessageDefinition, childMessages)
        {
            Factory = factory;
        }

        public StreamFactoryCreatedMessage(StreamFactory factory, IEnumerable<Message> predecessorMessages,
                                           params Message[] childMessages)
            : base(predecessorMessages, StreamFactoryCreatedMessageDefinition, childMessages)
        {
            Factory = factory;
        }

        public StreamFactory Factory { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }

        public void Dispose()
        {
            (Factory as IDisposable)?.Dispose();
        }
    }
}
