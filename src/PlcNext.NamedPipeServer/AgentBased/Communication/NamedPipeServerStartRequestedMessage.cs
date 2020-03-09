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
    public class NamedPipeServerStartRequestedMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition NamedPipeServerStartRequestedMessageDefinition { get; } =
            new MessageDefinition(nameof(NamedPipeServerStartRequestedMessage));

        #endregion

        public NamedPipeServerStartRequestedMessage(string address, Message predecessorMessage,
                                                    params Message[] childMessages)
            : base(predecessorMessage, NamedPipeServerStartRequestedMessageDefinition, childMessages)
        {
            Address = address;
        }

        public NamedPipeServerStartRequestedMessage(string address, IEnumerable<Message> predecessorMessages,
                                                    params Message[] childMessages)
            : base(predecessorMessages, NamedPipeServerStartRequestedMessageDefinition, childMessages)
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
