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
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(DeclarationFound))]
    [Produces(typeof(IdentifiersParsed))]
    internal class IdentifierParser : Agent
    {
        public IdentifierParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            DeclarationFound declarationFound = messageData.Get<DeclarationFound>();
            OnMessage(new IdentifiersParsed(messageData,GetIdentifier()));
            
            ParseNode[] GetIdentifier()
            {
                ParseNode content = declarationFound.Declaration.ChildrenSkipUnnamed().FirstOrDefault(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
                if (content == null)
                {
                    return Array.Empty<ParseNode>();
                }

                return content.ChildrenSkipUnnamed()
                              .TakeWhile(n => !FieldParser.EqualsMatch.IsMatch(n.ToString()))
                              .Select(Identifier).Where(n => n != null)
                              .ToArray();

                ParseNode Identifier(ParseNode parent)
                {
                    if (parent.RuleType == "choice" && parent.RuleName == "node")
                    {
                        ParseNode result = parent.FirstOrDefault();
                        if (result?.RuleName == "identifier" ||
                            result?.RuleName == "generic")
                        {
                            return result;
                        }
                    }

                    return null;
                }
            }
        }
    }
}
