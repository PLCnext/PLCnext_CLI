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
    [Produces(typeof(UsingsParsed))]
    internal class UsingsParser : Agent
    {
        public UsingsParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CppStreamParsedSuccessfully parsedSuccessfully = messageData.Get<CppStreamParsedSuccessfully>();
            string[] usings = GetUsings();
            OnMessage(new UsingsParsed(messageData, usings));
            
            string[] GetUsings()
            {
                List<string> result = new List<string>();
                foreach (ParseNode usingNode in parsedSuccessfully.Root.GetHierarchy().Where(n => n.RuleType == "leaf" &&
                                                                              n.RuleName == "identifier" &&
                                                                              n.ToString() == "using"))
                {
                    ParseNode declarationParent = usingNode.GetDeclarationContentParent();
                    string[] identifier = declarationParent.ChildrenSkipUnnamed()
                                                           .Select(i => i.Identifier())
                                                           .Where(i => i != null)
                                                           .Select(i => i.ToString())
                                                           .ToArray();
                    if (identifier.Length > 2 && identifier[0] == "using" && identifier[1] == "namespace")
                    {
                        result.Add(identifier.Skip(2).Aggregate(string.Empty, (s, s1) => s + s1));
                    }
                }

                return result.ToArray();
            }
        }
    }
}
