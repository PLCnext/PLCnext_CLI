#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(TypeDeclarationFound))]
    [Consumes(typeof(NameParsed))]
    [Consumes(typeof(UsingsParsed))]
    [Produces(typeof(BaseTypesParsed))]
    public class BaseTypeParser : Agent
    {
        private readonly MessageCollector<TypeDeclarationFound, NameParsed, UsingsParsed> collector;
        public BaseTypeParser(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<TypeDeclarationFound, NameParsed, UsingsParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<TypeDeclarationFound, NameParsed, UsingsParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            
            IEnumerable<(string, string)> baseTypeNames = set.Message1.Declaration.GetBaseTypes();
            CppDataType[] baseTypes = baseTypeNames.Select(n => new CppDataType(n.Item1, set.Message3.Usings.ToArray(), set.Message2.Namespace, n.Item2)).ToArray();
            OnMessage(new BaseTypesParsed(set, baseTypes));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
