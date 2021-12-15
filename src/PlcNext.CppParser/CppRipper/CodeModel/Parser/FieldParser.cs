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
using System.Text.RegularExpressions;

namespace PlcNext.CppParser.CppRipper.CodeModel.Parser
{
    internal static class FieldParser
    {
        public static CppDataType GetFieldDataType(this ParseNode[] typeNodes, string[] usings, string ns)
        {
            string dataTypeName = typeNodes.Aggregate(string.Empty, (s, node) => $"{s}{node}");
            CppDataType dataType = new CppDataType(dataTypeName, usings, ns);
            return dataType;
        }
        
        public static string[] GetFieldMultiplicity(this ParseNode identifier)
        {
            identifier = identifier.SkipUnnamedParents();
            int index = identifier.GetParentIndex();
            ParseNode parent = identifier.GetParent();
            List<string> multiplicities = new List<string>();
            bool firstMultiplicityFound = false;
            foreach (ParseNode sibling in parent.Skip(index + 1).SkipUnnamed())
            {
                if (sibling.RuleType != "choice" || sibling.RuleName != "node")
                {
                    continue;
                }

                ParseNode child = sibling.FirstOrDefault();
                if (child?.RuleType == "sequence" && child.RuleName == "bracketed_group")
                {
                    string bracketGroup = child.ToString().Trim();
                    multiplicities.Add(bracketGroup.Substring(1, bracketGroup.Length - 2));
                    firstMultiplicityFound = true;
                }
                else if (firstMultiplicityFound)
                {
                    break;
                }
            }

            return multiplicities.ToArray();
        }
        
        public static CppComment[] GetFieldComments(this ParseNode declaration)
        {
            ParseNode content = declaration.ChildrenSkipUnnamed().First(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
            return content.ChildrenSkipUnnamed()
                          .ToArray()
                          .SkipAfterLastVisibilityGroup()
                          .Where(IsComment)
                          .Where(c => !string.IsNullOrEmpty(c.ToString()))
                          .Select(CppComment.Parse)
                          .ToArray();

            bool IsComment(ParseNode node)
            {
                return node.RuleType == "sequence" && node.RuleName == "comment_set";
            }
        }
        
        public static ParseNode[] GetFieldTypeNodes(this ParseNode declaration, ParseNode[] identifiers)
        {
            return GetTypeDeclarationName() ?? GetNodes(identifiers);
            
            ParseNode[] GetTypeDeclarationName()
            {
                ParseNode typeDeclaration = declaration.GetHierarchy().FirstOrDefault(n => n.RuleName == "type_decl");
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

            ParseNode[] GetNodes(ParseNode[] parseNodes)
            {
                return parseNodes.TakeWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal))
                                 .Concat(parseNodes.SkipWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal)).Take(1))
                                 .ToArray();
            }
        }
        
        public static bool IsValidFieldDeclaration(this ParseNode declaration)
        {
            return !declaration.GetHierarchy().Any(IsForbiddenParanthesisGroup) &&
                   declaration.GetHierarchy().All(n => n.RuleName != "typedef_decl") &&
                   declaration.GetHierarchy().All(n => n.RuleName != "pp_directive");
            
            bool IsForbiddenParanthesisGroup(ParseNode n)
            {
                if (n.RuleName != "paran_group")
                {
                    return false;
                }

                ParseNode parent = n.GetParent();
                while (parent.Count(c => c.RuleName != "comment_set") == 1)
                {
                    n = parent;
                    parent = n.GetParent();
                }

                return parent.TakeWhile(c => c != n).All(c => !EqualsMatch.IsMatch(c.ToString()));
            }
        }
        public static ParseNode[] GetFieldIdentifier(this ParseNode declaration)
        {
            ParseNode content = declaration.ChildrenSkipUnnamed().FirstOrDefault(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
            if (content == null)
            {
                return Array.Empty<ParseNode>();
            }

            return content.ChildrenSkipUnnamed()
                          .TakeWhile(n => !EqualsMatch.IsMatch(n.ToString()))
                          .Select(Identifier)
                          .Where(n => n != null)
                          .ToArray();

            ParseNode Identifier(ParseNode parent)
            {
                if (parent.RuleType == "choice" && parent.RuleName == "node")
                {
                    ParseNode result = parent.FirstOrDefault();
                    if (result?.RuleName == "identifier" || result?.RuleName == "generic")
                    {
                        return result;
                    }
                }

                return null;
            }
        }
        
        public static ParseNode[] GetFieldValue(this ParseNode declaration)
        {
            ParseNode content = declaration.ChildrenSkipUnnamed().FirstOrDefault(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
            if (content == null)
            {
                return Array.Empty<ParseNode>();
            }

            return content.ChildrenSkipUnnamed()
                          .SkipWhile(n => !EqualsMatch.IsMatch(n.ToString()))
                          .Skip(1)
                          .Where(n => n.ToString() != ";")
                          .ToArray();
        }

        public static readonly Regex EqualsMatch = new Regex("^\\s?=\\s?$", RegexOptions.Compiled);
    }
}