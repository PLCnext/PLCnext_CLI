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
using System.Globalization;
using System.Linq;
using System.Reflection;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppSymbol : ISymbol
    {
        private CppSymbol(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static CppSymbol ParseSymbol(ParseNode[] content, CppSymbol lastSymbol, List<CppSymbol> symbols,
                                            string ns, string parentName)
        {
            string name = string.Join(string.Empty, content.Select(c => c.ToString()));
            string value = string.Empty;
            ParseNode valueSplitter = content.FirstOrDefault(c => c.GetHierarchy()
                                                                   .FirstOrDefault(n => n.RuleName == "symbol")
                                                                  ?.ToString().Contains("=") == true);
            if (valueSplitter != null && Array.IndexOf(content,valueSplitter) == 1)
            {
                name = content[0].ToString();
                value = string.Join(string.Empty, content.Skip(2).Select(c => c.ToString()));
            }
            else if (content.Length == 1)
            {
                name = content[0].ToString();
            }

            if (string.IsNullOrEmpty(value))
            {
                value = lastSymbol != null
                            ? long.TryParse(lastSymbol.Value, out long numberValue)
                                  ? (numberValue + 1).ToString("D", CultureInfo.InvariantCulture)
                                  : "0"
                            : "0";
            }

            value = value.Trim().Trim('\'', '"');
            if (!long.TryParse(value, out _))
            {
                try
                {
                    long convertedValue = Convert.ToInt64(value, 16);
                    value = convertedValue.ToString("D", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    IEnumerable<ParseNode> valueParts = content.Skip(2);
                    if (valueParts.Any())
                    {
                        value = ResolveValue(valueParts);
                    }
                }
            }
            return new CppSymbol(name, value);

            bool ContainsDeclarationContent(ParseNode node)
            {
                return node.GetHierarchy().FirstOrDefault(n => n.RuleName == "declaration_content") != null;
            }

            string ResolveValue(IEnumerable<ParseNode> valueParts)
            {
                string resolvedString = string.Empty;
                foreach (ParseNode node in valueParts)
                {
                    if (ContainsDeclarationContent(node))
                    {
                        resolvedString += ResolveValue(node.GetHierarchy()
                                                          .LastOrDefault(n => n.RuleName == "declaration_content"));
                        continue;
                    }

                    string current = node.GetHierarchy().FirstOrDefault(n => n.RuleName == "identifier")?.ToString().Trim().Trim('\'', '"');
                    if (string.IsNullOrEmpty(current))
                    {
                        ParseNode literal = node.GetHierarchy().FirstOrDefault(n => n.RuleName == "literal");
                        if (literal != null)
                        {
                            resolvedString += literal.ChildrenSkipUnnamed().FirstOrDefault().ToString() + " ";
                            continue;
                        }
                        resolvedString += node.ToString() + " ";
                        continue;
                    }

                    string[] parts = current.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

                    string prefix = current.Substring(0, current.Length - parts.Last().Length).Trim(':');
                    string elementName = parts.Last();
                    if ((ns + "::" + parentName).EndsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string tempValue = symbols.Where(s => s.Name == elementName).FirstOrDefault()?.Value;
                        if (string.IsNullOrEmpty(tempValue))
                        {
                            tempValue = elementName;
                        }
                        resolvedString += tempValue + " ";
                    }
                    else
                    {
                        throw new SymbolResolveException(current, ns + "::" + parentName);
                    }

                }
                resolvedString = resolvedString.Trim();
                if (!string.IsNullOrEmpty(resolvedString))
                {
                    try
                    {
                        object result = Calculator.Evaluate(resolvedString);
                        return result.ToString();
                    }
                    catch (EvaluationException)
                    {
                        return resolvedString;
                    }
                }
                return resolvedString;
            }
        }

        public string Name { get; }
        public string Value { get; }
    }
}
