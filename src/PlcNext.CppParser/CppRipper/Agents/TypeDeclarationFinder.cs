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
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(CppStreamParsedSuccessfully))]
    [Produces(typeof(TypeDeclarationFound))]
    [Produces(typeof(NoTypeDeclarationFound))]
    internal class TypeDeclarationFinder : Agent
    {
        public TypeDeclarationFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CppStreamParsedSuccessfully parsedSuccessfully = messageData.Get<CppStreamParsedSuccessfully>();
            TypeDeclarationFound[] messages = GetTypeDeclarations(parsedSuccessfully.Root)
                                             .Select(d => (declaration: d, content: d.GetDeclarationContentParent()))
                                             .Where(t => IsValidDeclaration(t.content, t.declaration))
                                             .Select(t => new TypeDeclarationFound(messageData, t.declaration, t.content))
                                             .ToArray();
            if (messages.Any())
            {
                OnMessages(messages);
            }
            else
            {
                OnMessage(new NoTypeDeclarationFound(messageData));
            }
            
            IEnumerable<ParseNode> GetTypeDeclarations(ParseNode current)
            {
                foreach (ParseNode node in current)
                {
                    if (node.RuleType == "sequence" && node.RuleName == "type_decl")
                    {
                        yield return node;
                    }
                    else
                    {
                        foreach (ParseNode child in GetTypeDeclarations(node))
                        {
                            yield return child;
                        }
                    }
                }
            }
            
            bool IsValidDeclaration(ParseNode content, ParseNode typeDeclaration)
            {
                return content.Count >= 2 &&
                       content.Any(c => c.GetHierarchy().Contains(typeDeclaration)) &&
                       content.SkipWhile(c => !c.GetHierarchy().Contains(typeDeclaration))
                              .Skip(1).Any(c => c.GetHierarchy().Any(n => n.RuleName == "brace_group"));
            }
        }
    }
}
