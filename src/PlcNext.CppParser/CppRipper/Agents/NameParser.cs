#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using Agents.Net;
using PlcNext.Common.Tools;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(TypeDeclarationFound))]
    [Produces(typeof(NameParsed))]
    internal class NameParser : Agent
    {
        public NameParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            TypeDeclarationFound declarationFound = messageData.Get<TypeDeclarationFound>();
            
            string typeName = GetName();
            string ns = GetNamespace();
            
            OnMessage(new NameParsed(messageData, typeName, ns));

            string GetName()
            {
                ParseNode leaf = declarationFound.Declaration.GetHierarchy()
                                                .FirstOrDefault(n => n.RuleName == "identifier" ||
                                                                     n.RuleName == "generic");
                if (leaf == null)
                {
                    return Guid.NewGuid().ToByteString();
                }

                return leaf.ToString();
            }

            string GetNamespace()
            {
                ParseNode declarationParent;
                ParseNode typeDeclaration = declarationFound.Declaration.GetDeclarationContentParent().GetParent();
                string result = string.Empty;
                while ((declarationParent = typeDeclaration.GetDeclarationContentParent()) != null)
                {
                    string[] identifier = declarationParent.ChildrenSkipUnnamed()
                                                           .Where(r => r.RuleName != "comment_set")
                                                           .Select(n => n.Identifier())
                                                           .TakeWhile(i => i != null)
                                                           .Select(i => i.ToString())
                                                           .ToArray();
                    if (identifier.Length > 1 && identifier[0] == "namespace")
                    {
                        result = $"{identifier.Skip(1).Aggregate(string.Empty, (s, s1) => s + s1)}::{result}";
                    }
                    else if (identifier.Length == 0)
                    {
                        result = FindIdentifierThroughParent();
                    }

                    typeDeclaration = declarationParent.GetParent();
                }

                if (!string.IsNullOrEmpty(result))
                {
                    result = result.Substring(0, result.Length - 2);
                }

                return result;

                string FindIdentifierThroughParent()
                {
                    ParseNode parentTypeDeclaration = declarationParent.ChildrenSkipUnnamed()
                                                                       .Where(c => c.RuleType == "choice" &&
                                                                                  c.RuleName == "node")
                                                                       .SelectMany(c => c.ChildrenSkipUnnamed())
                                                                       .FirstOrDefault(
                                                                            c => c.RuleType == "sequence" &&
                                                                                c.RuleName == "type_decl");
                    ParseNode name = parentTypeDeclaration?.GetHierarchy()
                                                           .FirstOrDefault(n => n.RuleType == "leaf" &&
                                                                               n.RuleName == "identifier");
                    if (name != null)
                    {
                        result = $"{name}::{result}";
                    }

                    return result;
                }
            }
        }
    }
}
