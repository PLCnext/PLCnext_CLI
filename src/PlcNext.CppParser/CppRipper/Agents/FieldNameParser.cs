#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(IdentifiersParsed))]
    [Consumes(typeof(DataTypeParsed))]
    [Produces(typeof(FieldNamesParsed))]
    public class FieldNameParser : Agent
    {
        private readonly MessageCollector<IdentifiersParsed, DataTypeParsed> collector;
        public FieldNameParser(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<IdentifiersParsed, DataTypeParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<IdentifiersParsed, DataTypeParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);

            string[] names = set.Message1.Identifiers.Except(set.Message2.TypeNodes)
                                .Select(i => i.ToString())
                                .ToArray();
            OnMessage(new FieldNamesParsed(set, names));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
