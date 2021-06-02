#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(ParserError))]
    [Consumes(typeof(CppFileFound))]
    [Produces(typeof(CppFileResult))]
    public class ParserErrorHandler : Agent
    {
        private readonly MessageCollector<ParserError, CppFileFound> collector;
        public ParserErrorHandler(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<ParserError, CppFileFound>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<ParserError, CppFileFound> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            OnMessage(new CppFileResult(set, new FileResult(set.Message1.ErrorMessage, set.Message2.File, set.Message2.Directory)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
