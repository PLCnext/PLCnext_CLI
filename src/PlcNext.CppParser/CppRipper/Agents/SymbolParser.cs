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
    [Consumes(typeof(EnumFound))]
    [Consumes(typeof(TypeDeclarationFound), Implicitly = true)]
    [Produces(typeof(SymbolsParsed))]
    internal class SymbolParser : Agent
    {
        public SymbolParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            TypeDeclarationFound typeDeclarationFound = messageData.Get<TypeDeclarationFound>();
            OnMessage(new SymbolsParsed(messageData, ParseSymbols().ToArray()));
            
            IEnumerable<CppSymbol> ParseSymbols()
            {
                ParseNode declarationContent = typeDeclarationFound.Content.GetHierarchy().FirstOrDefault(n => n.RuleName == "declaration_content");
                ParseNode[] allSymbols = declarationContent.ChildrenSkipUnnamed().Where(n => n.RuleName != "comment_set").ToArray();
                CppSymbol lastSymbol = null;
                foreach (ParseNode[] symbol in SplitSymbols())
                {
                    yield return lastSymbol = CppSymbol.ParseSymbol(symbol, lastSymbol);
                }

                IEnumerable<ParseNode[]> SplitSymbols()
                {
                    List<ParseNode[]> groups = new List<ParseNode[]>();
                    List<ParseNode> nodes = new List<ParseNode>(allSymbols);
                    ParseNode comma = FindFirstComma();
                    while (comma != null)
                    {
                        comma = FindFirstComma();
                        int index = nodes.IndexOf(comma);
                        ParseNode[] group = nodes.Take(index).ToArray();
                        if (group.Any())
                        {
                            groups.Add(group);
                        }

                        nodes.RemoveRange(0, index + 1);
                    }

                    if (nodes.Any())
                    {
                        groups.Add(nodes.ToArray());
                    }

                    return groups;

                    ParseNode FindFirstComma()
                    {
                        return nodes.FirstOrDefault(n => n.GetHierarchy()
                                                          .FirstOrDefault(t => t.RuleName == "symbol")
                                                         ?.ToString().Contains(",") == true);
                    }
                }
            }
        }
    }
}
