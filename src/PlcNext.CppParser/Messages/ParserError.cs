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
using PlcNext.CppParser.CppRipper;

namespace PlcNext.CppParser.Messages
{
    internal class ParserError : Message
    {
        public ParserError(Message predecessorMessage, ParserMessage errorMessage): base(predecessorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ParserError(IEnumerable<Message> predecessorMessages, ParserMessage errorMessage): base(predecessorMessages)
        {
            ErrorMessage = errorMessage;
        }
        
        public ParserMessage ErrorMessage { get; }

        protected override string DataToString()
        {
            return $"{nameof(ErrorMessage)}: {ErrorMessage}";
        }
    }
}
