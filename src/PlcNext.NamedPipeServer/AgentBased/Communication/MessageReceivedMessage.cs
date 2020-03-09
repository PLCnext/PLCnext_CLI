#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agents.Net;

namespace PlcNext.NamedPipeServer.AgentBased.Communication
{
    public class MessageReceivedMessage : ConsumableMessage
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition MessageReceivedMessageDefinition { get; } =
            new MessageDefinition(nameof(MessageReceivedMessage));

        #endregion

        public MessageReceivedMessage(Stream message, params Message[] childMessages)
            : base(Enumerable.Empty<Message>(), MessageReceivedMessageDefinition, childMessages)
        {
            Message = message;
        }

        public Stream Message { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }

        protected override void DisposeConsumables()
        {
            Message.Dispose();
        }
    }
}
