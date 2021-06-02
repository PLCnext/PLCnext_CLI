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

namespace PlcNext.CppParser.CppRipper.Messages
{
    public class CppStreamParsedSuccessfully : Message
    {
        public CppStreamParsedSuccessfully(Message predecessorMessage, ParseNode root): base(predecessorMessage)
        {
            Root = root;
        }

        public CppStreamParsedSuccessfully(IEnumerable<Message> predecessorMessages, ParseNode root): base(predecessorMessages)
        {
            Root = root;
        }

        public ParseNode Root { get; }
        
        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
