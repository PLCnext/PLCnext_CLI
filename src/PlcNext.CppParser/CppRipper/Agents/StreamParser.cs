#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using Agents.Net;
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(CppFileFound))]
    [Consumes(typeof(CppFileOpened), Implicitly = true)]
    [Produces(typeof(CppStreamParsedSuccessfully))]
    [Produces(typeof(ParserError))]
    internal class StreamParser : Agent
    {
        public StreamParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CppFileOpened opened = messageData.Get<CppFileOpened>();
            CppStreamParser parser = new CppStreamParser();
            ParseNode root = parser.Parse(opened.FileStream);
            if (parser.Succeeded)
            {
                OnMessage(new CppStreamParsedSuccessfully(messageData, root));
            }
            else
            {
                ParserMessage message = parser.Exception == null
                                            ? new ParserMessage("CPP0004", 1, 1)
                                            : new ParserMessage("CPP0005", parser.Exception.Row, parser.Exception.Col,
                                                                $"{Environment.NewLine}{parser.Exception.Message}");
                OnMessage(new ParserError(messageData, message));
            }
        }
    }
}
