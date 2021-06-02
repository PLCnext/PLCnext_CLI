#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(EnumFound))]
    [Consumes(typeof(TypeDeclarationFound), Implicitly = true)]
    [Consumes(typeof(NameParsed))]
    [Consumes(typeof(TypeFieldsAggregated))]
    [Consumes(typeof(SymbolsParsed))]
    [Consumes(typeof(CommentsParsed))]
    [Consumes(typeof(BaseTypesParsed))]
    [Consumes(typeof(AttributePrefixDefined))]
    [Produces(typeof(EnumCreated))]
    public class EnumConstructor : Agent
    {
        private readonly MessageCollector<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed,
            AttributePrefixDefined, SymbolsParsed> collector;
        public EnumConstructor(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector =
                new MessageCollector<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed,
                    AttributePrefixDefined, SymbolsParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed, AttributePrefixDefined, SymbolsParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);
            set.MarkAsConsumed(set.Message4);
            set.MarkAsConsumed(set.Message5);
            set.MarkAsConsumed(set.Message7);

            CodePosition codePosition =
                new CodePosition(set.Message1.Declaration.Position.line, set.Message1.Declaration.Position.column);
            CppEnum cppEnum = new CppEnum(set.Message4.Namespace, set.Message4.Name,
                                          set.Message2.Comments, set.Message3.Fields,
                                          set.Message5.BaseTypes, set.Message6.AttributePrefix,
                                          set.Message7.Symbols);
            OnMessage(EnumCreated.Decorate(new TypeCreated(set, cppEnum, codePosition), cppEnum));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
