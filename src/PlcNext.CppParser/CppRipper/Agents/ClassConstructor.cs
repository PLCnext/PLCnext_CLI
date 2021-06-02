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
    [Consumes(typeof(ClassFound))]
    [Consumes(typeof(TypeDeclarationFound), Implicitly = true)]
    [Consumes(typeof(CommentsParsed))]
    [Consumes(typeof(TypeFieldsAggregated))]
    [Consumes(typeof(NameParsed))]
    [Consumes(typeof(BaseTypesParsed))]
    [Consumes(typeof(AttributePrefixDefined))]
    [Produces(typeof(ClassCreated))]
    public class ClassConstructor : Agent
    {
        private readonly MessageCollector<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed,
            AttributePrefixDefined> collector;
        public ClassConstructor(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector =
                new MessageCollector<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed,
                    AttributePrefixDefined>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<TypeDeclarationFound, CommentsParsed, TypeFieldsAggregated, NameParsed, BaseTypesParsed, AttributePrefixDefined> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);
            set.MarkAsConsumed(set.Message4);
            set.MarkAsConsumed(set.Message5);

            CodePosition codePosition =
                new CodePosition(set.Message1.Declaration.Position.line, set.Message1.Declaration.Position.column);
            CppClass cppClass = new CppClass(set.Message4.Namespace, set.Message4.Name,
                                             set.Message2.Comments, set.Message3.Fields,
                                             set.Message5.BaseTypes, set.Message6.AttributePrefix);
            OnMessage(ClassCreated.Decorate(new TypeCreated(set, cppClass, codePosition), cppClass));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
