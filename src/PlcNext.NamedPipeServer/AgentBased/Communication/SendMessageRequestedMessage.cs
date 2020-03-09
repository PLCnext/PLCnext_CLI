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
using System.IO;
using Agents.Net;

namespace PlcNext.NamedPipeServer.AgentBased.Communication
{
    public class SendMessageRequestedMessage : ConsumableMessage
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition SendMessageRequestedMessageDefinition { get; } =
            new MessageDefinition(nameof(SendMessageRequestedMessage));

        #endregion

        public SendMessageRequestedMessage(Stream message, Action messageCompletedAction, Message predecessorMessage,
                                  params Message[] childMessages)
            : base(predecessorMessage, SendMessageRequestedMessageDefinition, childMessages)
        {
            Message = message;
            MessageCompletedAction = messageCompletedAction;
        }

        public SendMessageRequestedMessage(Stream message, Action messageCompletedAction,
                                  IEnumerable<Message> predecessorMessages, params Message[] childMessages)
            : base(predecessorMessages, SendMessageRequestedMessageDefinition, childMessages)
        {
            Message = message;
            MessageCompletedAction = messageCompletedAction;
        }

        public Stream Message { get; }

        public Action MessageCompletedAction { get; }

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
