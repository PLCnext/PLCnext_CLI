#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Globalization;
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(FieldNamesParsed))]
    [Consumes(typeof(MultiplicitiesParsed))]
    [Consumes(typeof(DataTypeParsed))]
    [Consumes(typeof(FieldSetCommentsParsed))]
    [Consumes(typeof(AttributePrefixDefined))]
    [Consumes(typeof(DeclarationFound))]
    [Produces(typeof(FieldSetConstructed))]
    public class FieldSetConstructor : Agent
    {
        private readonly MessageCollector<FieldNamesParsed, MultiplicitiesParsed, DataTypeParsed, FieldSetCommentsParsed
          , AttributePrefixDefined, DeclarationFound> collector;
        public FieldSetConstructor(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector =
                new MessageCollector<FieldNamesParsed, MultiplicitiesParsed, DataTypeParsed, FieldSetCommentsParsed,
                    AttributePrefixDefined, DeclarationFound>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<FieldNamesParsed, MultiplicitiesParsed, DataTypeParsed, FieldSetCommentsParsed, AttributePrefixDefined, DeclarationFound> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);
            set.MarkAsConsumed(set.Message4);
            set.MarkAsConsumed(set.Message6);

            CppField[] fields = set.Message1.FieldNames
                                   .Select((name, i) => new CppField(name, set.Message3.DataType,
                                                                     set.Message4.Comments.ToArray(),
                                                                     set.Message2.Multiplicities[i].Select(m => m.ToString(CultureInfo.InvariantCulture)).ToArray(),
                                                                     set.Message5.AttributePrefix))
                                   .ToArray();
            OnMessage(new FieldSetConstructed(set, fields, set.Message6.FileIndex));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
