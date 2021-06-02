#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(IdentifiersParsed))]
    [Consumes(typeof(UsingsParsed))]
    [Consumes(typeof(DeclarationFound))]
    [Consumes(typeof(NameParsed))]
    [Produces(typeof(DataTypeParsed))]
    public class TypeFinder : Agent
    {
        private readonly MessageCollector<IdentifiersParsed, UsingsParsed, DeclarationFound, NameParsed> collector;
        public TypeFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<IdentifiersParsed, UsingsParsed, DeclarationFound, NameParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<IdentifiersParsed, UsingsParsed, DeclarationFound, NameParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message3);
            
            ParseNode[] typeNodes = GetTypeDeclarationName() ?? GetTypeNodes(set.Message1.Identifiers);
            string dataTypeName = typeNodes.Aggregate(string.Empty,(s, node) => $"{s}{node}");
            string fieldNamespace = $"{set.Message4.Namespace}::{set.Message4.Name}";
            CppDataType dataType = new CppDataType(dataTypeName, set.Message2.Usings.ToArray(), fieldNamespace);
            OnMessage(new DataTypeParsed(set, typeNodes,set.Message3.Declaration, dataType));

            ParseNode[] GetTypeDeclarationName()
            {
                ParseNode typeDeclaration = set.Message3.Declaration.GetHierarchy().FirstOrDefault(n => n.RuleName == "type_decl");
                return typeDeclaration?.GetHierarchy().Select(Identifier).Where(n => n != null).ToArray();

                ParseNode Identifier(ParseNode node)
                {
                    if (node.RuleName == "identifier" ||
                        node.RuleName == "generic")
                    {
                        return node;
                    }

                    return null;
                }
            }

            ParseNode[] GetTypeNodes(IReadOnlyCollection<ParseNode> parseNodes)
            {
                return parseNodes.TakeWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal))
                                 .Concat(parseNodes.SkipWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal)).Take(1))
                                 .ToArray();
            }
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
