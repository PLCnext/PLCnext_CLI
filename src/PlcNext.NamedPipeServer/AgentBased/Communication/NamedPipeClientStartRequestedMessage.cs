#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;

namespace PlcNext.NamedPipeServer.AgentBased.Communication
{
    public class NamedPipeClientStartRequestedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition NamedPipeClientStartRequestedMessageDefinition { get; } =
            new MessageDefinition(nameof(NamedPipeClientStartRequestedMessage));

        #endregion

        public NamedPipeClientStartRequestedMessage(string address, Message predecessorMessage,
                                                    params Message[] childMessages)
            : base(predecessorMessage, NamedPipeClientStartRequestedMessageDefinition, childMessages)
        {
            Address = address;
        }

        public NamedPipeClientStartRequestedMessage(string address, IEnumerable<Message> predecessorMessages,
                                                    params Message[] childMessages)
            : base(predecessorMessages, NamedPipeClientStartRequestedMessageDefinition, childMessages)
        {
            Address = address;
        }

        public string Address { get; }

        protected override string DataToString()
        {
            return $"{nameof(Address)}: {Address}";
        }
    }
}
