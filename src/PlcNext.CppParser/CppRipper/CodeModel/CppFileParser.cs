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
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    internal class CppFileParser : IFileParser
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly ILog log;

        public CppFileParser(ISettingsProvider settingsProvider, ILog log)
        {
            this.settingsProvider = settingsProvider;
            this.log = log;
        }

        public ParserResult Parse(VirtualFile file)
        {
            log.LogVerbose($"Parse file {file.FullName}.");
            List<CodeSpecificException> codeSpecificExceptions = new List<CodeSpecificException>();
            List<ParserMessage> messages = new List<ParserMessage>();
            IDictionary<IType, CodePosition> types = new Dictionary<IType, CodePosition>();
            CppStreamParser parser = new CppStreamParser();
            ParseNode root = parser.Parse(file.OpenRead());
            if (!parser.Succeeded)
            {
                messages.Add(parser.Exception == null
                                 ? new ParserMessage("CPP0004", 1, 1)
                                 : new ParserMessage("CPP0005", parser.Exception.Row, parser.Exception.Col,
                                                     $"{Environment.NewLine}{parser.Exception.Message}"));
                codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
                return new ParserResult(codeSpecificExceptions);
            }

            string[] usings = GetUsings();
            foreach (ParseNode typeDeclaration in GetTypeDeclarations(root))
            {
                ParseNode content = GetDeclarationContentParent(typeDeclaration);
                if (content != null && IsValidDecalration(content, typeDeclaration))
                {
                    string name = GetName(typeDeclaration);
                    string ns = GetNamespace(typeDeclaration);
                    switch (typeDeclaration[1].RuleName)
                    {
                        case "struct_decl":
                            CppStructure structure = new CppStructure(ns, name, usings, content, messages, typeDeclaration,
                                                                      settingsProvider.Settings.AttributePrefix);
                            types.Add(structure, new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                            break;
                        case "class_decl":
                            CppClass cppClass = new CppClass(ns, name, usings, content, typeDeclaration[1], messages,
                                                             settingsProvider.Settings.AttributePrefix);
                            types.Add(cppClass, new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                            break;
                        case "enum_decl":
                            CppEnum cppEnum = new CppEnum(ns, name, usings, content, messages, typeDeclaration[1],
                                                          settingsProvider.Settings.AttributePrefix);
                            types.Add(cppEnum, new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                            break;
                        default:
                            //do nothing
                            break;
                    }
                }
            }

            codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
            string[] includes = GetIncludes();
            return new ParserResult(codeSpecificExceptions, types, includes);

            bool IsValidDecalration(ParseNode content, ParseNode typeDeclaration)
            {
                return content.Count >= 2 &&
                       content.Any(c => c.GetHierarchy().Contains(typeDeclaration)) &&
                       content.SkipWhile(c => !c.GetHierarchy().Contains(typeDeclaration))
                              .Skip(1).Any(c => c.GetHierarchy().Any(n => n.RuleName == "brace_group"));
            }

            string GetName(ParseNode typeDeclaration)
            {
                ParseNode leaf = typeDeclaration.GetHierarchy()
                                                .FirstOrDefault(n => n.RuleName == "identifier" ||
                                                                     n.RuleName == "generic");
                if (leaf == null)
                {
                    return Guid.NewGuid().ToByteString();
                }

                return leaf.ToString();
            }

            ParseNode GetDeclarationContentParent(ParseNode current)
            {
                while (current != null &&
                       current.RuleType != "plus" &&
                       current.RuleName != "declaration_content")
                {
                    current = current.GetParent();
                }

                return current;
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

            string[] GetIncludes()
            {
                List<string> result = new List<string>();
                foreach (ParseNode includeNode in root.GetHierarchy().Where(n => n.RuleType == "sequence" &&
                                                                                 n.RuleName == "pp_directive" &&
                                                                                 n.Any(c => c.ToString().Equals("include", StringComparison.OrdinalIgnoreCase))))
                {
                    ParseNode include = includeNode.FirstOrDefault(n => n.RuleName == "until_eol");
                    if (include != null)
                    {
                        result.Add(include.ToString().Trim('\"'));
                    }
                }

                return result.ToArray();
            }

            string[] GetUsings()
            {
                List<string> result = new List<string>();
                foreach (ParseNode usingNode in root.GetHierarchy().Where(n => n.RuleType == "leaf" &&
                                                                               n.RuleName == "identifier" &&
                                                                               n.ToString() == "using"))
                {
                    ParseNode declarationParent = GetDeclarationContentParent(usingNode);
                    string[] identifier = declarationParent.ChildrenSkipUnnamed()
                                                           .Select(Identifier)
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

            ParseNode Identifier(ParseNode parent)
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

            string GetNamespace(ParseNode typeDeclaration)
            {
                ParseNode declarationParent;
                typeDeclaration = GetDeclarationContentParent(typeDeclaration).GetParent();
                string result = string.Empty;
                while ((declarationParent = GetDeclarationContentParent(typeDeclaration)) != null)
                {
                    string[] identifier = declarationParent.ChildrenSkipUnnamed()
                                                           .Where(r => r.RuleName != "comment_set")
                                                           .Select(Identifier)
                                                           .TakeWhile(i => i != null)
                                                           .Select(i => i.ToString())
                                                           .ToArray();
                    if (identifier.Length > 1 && identifier[0] == "namespace")
                    {
                        result = $"{identifier.Skip(1).Aggregate(string.Empty, (s, s1) => s + s1)}::{result}";
                    }
                    else if (identifier.Length == 0)
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
                    }

                    typeDeclaration = declarationParent.GetParent();
                }

                if (!string.IsNullOrEmpty(result))
                {
                    result = result.Substring(0, result.Length - 2);
                }

                return result;
            }
        }
    }
}
