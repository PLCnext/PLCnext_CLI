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

namespace PlcNext.CppParser.CppRipper
{
    internal static class CppRipperExtensions
    {
        internal static IEnumerable<(string, string)> GetBaseTypes(this ParseNode declaration)
        {
            foreach (ParseNode baseTypeNode in declaration.GetHierarchy().Where(IsBaseType))
            {
                ParseNode identifier = baseTypeNode.First(n => n.RuleName == "identifier" || n.RuleName == "generic");

                ParseNode visibility = baseTypeNode.GetHierarchy().Where(IsVisibility)
                                                   .LastOrDefault();

                yield return (identifier.ToString(), visibility?.ToString().Trim() ?? "public");
            }

            bool IsVisibility(ParseNode typeNode)
            {
                return typeNode.RuleType == "choice" && typeNode.RuleName == "visibility";
            }

            bool IsBaseType(ParseNode typeNode)
            {
                return typeNode.RuleType == "sequence" && typeNode.RuleName == "base_type";
            }
        }

        internal static ParseNode Identifier(this ParseNode parent)
        {
            if (parent.RuleType == "choice" && parent.RuleName == "node")
            {
                ParseNode result = parent.FirstOrDefault();
                if (result?.RuleType == "leaf" && result.RuleName == "identifier")
                {
                    return result;
                }
            }

            return null;
        }

        internal static ParseNode GetDeclarationList(this ParseNode content, ParseNode typeDeclaration)
        {
            return (from node in
                        content.ChildrenSkipUnnamed()
                               .SkipWhile(c => !c.GetHierarchy().Contains(typeDeclaration))
                               .Where(c => c.RuleType == "choice" && c.RuleName == "node")
                    select node.FirstOrDefault()
                    into nodeContent
                    where nodeContent?.RuleType == "sequence" && nodeContent.RuleName == "brace_group"
                    select nodeContent.FirstOrDefault(n => n.RuleType == "recursive" && n.RuleName == "declaration_list"))
               .FirstOrDefault();
        }

        internal static IEnumerable<ParseNode> SkipAfterLastVisibilityGroup(this IReadOnlyCollection<ParseNode> nodes)
        {
            ParseNode lastVisibilityGroup = nodes.LastOrDefault(n => n.RuleName == "node" &&
                                                                     n.ChildrenSkipUnnamed().FirstOrDefault()
                                                                     ?.RuleName == "visibility_group");
            return lastVisibilityGroup != null
                       ? nodes.SkipWhile(n => n != lastVisibilityGroup)
                              .Skip(1)
                       : nodes;
        }
        
        internal static IEnumerable<ParseNode> ChildrenSkipUnnamed(this ParseNode parent)
        {
            foreach (ParseNode child in parent)
            {
                if (child.IsUnnamed())
                {
                    foreach (ParseNode grandChildren in child.ChildrenSkipUnnamed())
                    {
                        yield return grandChildren;
                    }
                }
                else
                {
                    yield return child;
                }
            }
        }

        internal static IEnumerable<ParseNode> SkipUnnamed(this IEnumerable<ParseNode> children)
        {
            foreach (ParseNode child in children)
            {
                if (child.IsUnnamed())
                {
                    foreach (ParseNode grandChildren in child.ChildrenSkipUnnamed())
                    {
                        yield return grandChildren;
                    }
                }
                else
                {
                    yield return child;
                }
            }
        }

        internal static ParseNode SkipUnnamedParents(this ParseNode child)
        {
            while (child.GetParent()?.IsUnnamed() == true)
            {
                child = child.GetParent();
            }

            return child;
        }
    }
}
