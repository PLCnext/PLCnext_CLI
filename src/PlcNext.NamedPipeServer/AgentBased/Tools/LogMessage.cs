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

namespace PlcNext.NamedPipeServer.AgentBased.Tools
{
    public class LogMessage : Message
    {
        #region Definition

        [MessageDefinition]
        public static MessageDefinition LogMessageDefinition { get; } =
            new MessageDefinition(nameof(LogMessage));

        #endregion

        public LogMessage(string message, LogLevel level, Message predecessorMessage, params Message[] childMessages)
            : base(predecessorMessage, LogMessageDefinition, childMessages)
        {
            Message = message;
            Level = level;
        }

        public LogMessage(string message, LogLevel level, IEnumerable<Message> predecessorMessages,
                          params Message[] childMessages)
            : base(predecessorMessages, LogMessageDefinition, childMessages)
        {
            Message = message;
            Level = level;
        }

        public string Message { get; }
        public LogLevel Level { get; }

        protected override string DataToString()
        {
            return $"{nameof(Message)}: {Message}; {nameof(Level)}: {Level}";
        }
    }

    public enum LogLevel
    {
        Verbose,
        Information,
        Warning,
        Error
    }
}
