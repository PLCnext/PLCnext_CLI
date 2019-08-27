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
using System.Reflection;
using System.Text;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppEnum : CppType, IEnum
    {
        public CppEnum(string ns, string name, string[] usings, ParseNode content, List<ParserMessage> messages,
                       ParseNode typeDeclaration, string attributePrefix) : base(
            ns, name, usings, content, messages, typeDeclaration, attributePrefix, false)
        {
            ParseSymbols();

            void ParseSymbols()
            {
                ParseNode declarationContent = content.GetHierarchy().FirstOrDefault(n => n.RuleName == "declaration_content");
                ParseNode[] allSymbols = declarationContent.ChildrenSkipUnnamed().Where(n => n.RuleName != "comment_set").ToArray();
                CppSymbol lastSymbol = null;
                foreach (ParseNode[] symbol in SplitSymbols())
                {
                    lastSymbol = CppSymbol.ParseSymbol(symbol, lastSymbol);
                    symbols.Add(lastSymbol);
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

        private readonly List<CppSymbol> symbols = new List<CppSymbol>();
        public IEnumerable<ISymbol> Symbols => symbols;

        public override IEnumerable<IField> Fields => throw new InvalidOperationException("Use symbols instead.");
    }
}
