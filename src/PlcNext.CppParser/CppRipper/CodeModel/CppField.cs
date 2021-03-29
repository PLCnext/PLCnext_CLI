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
using PlcNext.Common.Tools;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppField : CppCodeEntity, IField
    {
        public CppField(string name, IDataType dataType, IComment[] comments, int[] multiplicity, string attributePrefix, IType containingType) : base(name, attributePrefix)
        {
            DataType = dataType;
            Comments = comments;
            Multiplicity = multiplicity;
            ContainingType = containingType;
        }

        public IDataType DataType { get; }
        public IReadOnlyCollection<int> Multiplicity { get; }
        public IType ContainingType { get; }

        public static IEnumerable<CppField> Parse(ParseNode declaration, string[] usings, string ns,
                                                  List<ParserMessage> messages, string attributePrefix,
                                                  CppType containingType)
        {
            if (declaration.GetHierarchy().Any(n => n.RuleName == "paran_group") ||
                declaration.GetHierarchy().Any(n => n.RuleName == "typedef_decl") ||
                declaration.GetHierarchy().Any(n => n.RuleName == "pp_directive"))
            {
                return Enumerable.Empty<CppField>();
            }

            ParseNode[] identifiers = GetIdentifier();
            if (identifiers.FirstOrDefault()?.ToString() == "using")
            {
                //using directive inside class/struct
                return Enumerable.Empty<CppField>();
            }
            ParseNode[] typeNodes = GetTypeDeclarationName() ?? GetTypeNodes(identifiers);
            if (identifiers.SequenceEqual(typeNodes))
            {
                if (typeNodes.Any())
                {
                    (int line, int column) position = declaration.Position;
                    messages.Add(new ParserMessage("CPP0001", position.line, position.column));
                }

                //Empty group "private:"
                return Enumerable.Empty<CppField>();
            }

            string dataTypeName = typeNodes.Aggregate(string.Empty,(s, node) => $"{s}{node}");
            IEnumerable<(string name, int[] multiplicity)> fields = identifiers.Except(typeNodes).Select(i => (i.ToString(), GetMultiplicity(i.GetParent())));
            IComment[] comments = GetComments();
            CppDataType dataType = new CppDataType(dataTypeName, usings, ns);

            return fields.Select(fd => new CppField(fd.name, dataType, comments, fd.multiplicity, attributePrefix, containingType));

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

            IComment[] GetComments()
            {
                ParseNode content = declaration.ChildrenSkipUnnamed().First(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
                return content.ChildrenSkipUnnamed().ToArray()
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

            ParseNode[] GetIdentifier()
            {
                ParseNode content = declaration.ChildrenSkipUnnamed().FirstOrDefault(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
                if (content == null)
                {
                    return Array.Empty<ParseNode>();
                }

                return content.ChildrenSkipUnnamed()
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

            int[] GetMultiplicity(ParseNode identifier)
            {
                identifier = identifier.SkipUnnamedParents();
                int index = identifier.GetParentIndex();
                ParseNode parent = identifier.GetParent();
                List<int> multiplicities = new List<int>();
                bool firstMultiplicityFound = false;
                foreach (ParseNode sibling in parent.Skip(index+1).SkipUnnamed())
                {
                    if (sibling.RuleType == "choice" && sibling.RuleName == "node")
                    {
                        ParseNode child = sibling.FirstOrDefault();
                        if (child?.RuleType == "sequence" && child.RuleName == "bracketed_group")
                        {
                            string bracketGroup = child.ToString().Trim();
                            if (int.TryParse(bracketGroup.Substring(1, bracketGroup.Length-2), out int result))
                            {
                                multiplicities.Add(result);
                                firstMultiplicityFound = true;
                            }
                        }
                        else if(firstMultiplicityFound)
                        {
                            break;
                        }
                    }
                }

                return multiplicities.ToArray();
            }

            ParseNode[] GetTypeNodes(ParseNode[] parseNodes)
            {
                return parseNodes.TakeWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal))
                                 .Concat(parseNodes.SkipWhile(n => n.ToString().EndsWith("::", StringComparison.Ordinal)).Take(1))
                                 .ToArray();
            }
        }
    }
}
