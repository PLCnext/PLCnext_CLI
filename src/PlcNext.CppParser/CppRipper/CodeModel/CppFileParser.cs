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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    internal sealed class CppFileParser : IFileParser
    {
        public static readonly Regex StatementParser = new Regex(@"^(?<Key>[^\s\(\)]+)\s+(?<Value>.+?)(?:\s*\/\/.*)?$",
                                                                 RegexOptions.Compiled);
        
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
            List<CodeSpecificException> codeSpecificExceptions = new();
            List<ParserMessage> messages = new();
            CppStreamParser parser = new();

            string text;
            using (StreamReader reader = new StreamReader(file.OpenRead()))
            {
                text = reader.ReadToEnd();
            }
            
            text = Regex.Replace(text, @"^\s*#(?!define|include)(?:(?!\*/|/\*).)+$", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            var stream = RecyclableMemoryStreamManager.Instance.GetStream();
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            
            ParseNode root = parser.Parse(stream);
            if (!parser.Succeeded)
            {
                messages.Add(parser.Exception == null
                                 ? new ParserMessage("CPP0004", 1, 1)
                                 : new ParserMessage("CPP0005", parser.Exception.Row, parser.Exception.Col,
                                                     $"{Environment.NewLine}{parser.Exception.Message}"));
                codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
                return new ParserResult(codeSpecificExceptions);
            }

            string[] usings = GetUsing(root);
            IDictionary<IType, CodePosition> types = GetTypes(root, usings, messages);
            string[] includes = GetIncludes(root);
            Dictionary<string, string> defineStatements = GetDefineStatements(root);
            IEnumerable<IConstant> constants = GetConstants(root, usings);

            codeSpecificExceptions.AddRange(messages.Select(m => m.ToException(file)));
            
            return new ParserResult(codeSpecificExceptions, types, includes, defineStatements, constants);
        }

        private List<IConstant> GetConstants(ParseNode root, string[] usings)
        {
            List<IConstant> constants = new List<IConstant>();
            IEnumerable<ParseNode> declarations = root.GetHierarchy()
                                                      .Where(n => n.RuleName == "declaration" &&
                                                                  n.RuleType == "choice" &&
                                                                  n.ToString().Contains("const ", StringComparison.Ordinal));
            foreach (ParseNode declaration in declarations)
            {
                ParseConstant(declaration);
            }

            return constants;

            void ParseConstant(ParseNode declaration)
            {
                ParseNode[] content = declaration.GetFieldDeclarationContent();
                if (content.IsValidFieldDeclaration() && ContainsConstIdentifier())
                {
                    
                }
                if (!content.IsValidFieldDeclaration() ||
                    !ContainsConstIdentifier() ||
                    IsPartOfMethodDefinition() ||
                    HasTypeDeclarationParent(declaration))
                {
                    return;
                }

//paran_group
//declaration_list
                ParseNode[] identifiers = content.GetFieldIdentifier()
                                                 .SkipWhile(i => i.ToString().Trim() != "const")
                                                 .Skip(1)
                                                 .ToArray();
                if (!identifiers.Any())
                {
                    return;
                }

                ParseNode[] typeNodes = content.GetFieldTypeNodes(identifiers);
                ParseNode[] valueNodes = content.GetFieldValue();
                if (identifiers.SequenceEqual(typeNodes))
                {
                    return;
                }

                string ns = GetNamespace(declaration, skipFirst:false);
                CppDataType dataType = typeNodes.GetFieldDataType(usings, ns);
                IEnumerable<string> fields = identifiers.SkipWhile(i => !typeNodes.Contains(i))
                                                        .SkipWhile(typeNodes.Contains)
                                                        .Where(i => !i.GetParent().GetFieldMultiplicity().Any())
                                                        .Select(i => i.ToString());
                IEnumerable<string> values = valueNodes.Where(i => !string.IsNullOrEmpty(i.ToString().Trim()))
                                                       .Select(i => i.ToString().Trim());
                string value = string.Join(" ", values);

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                foreach (string field in fields)
                {
                    constants.Add(new CppConstant(field, settingsProvider.Settings.AttributePrefix,
                                                  ns, value, dataType, usings));
                }

                bool ContainsConstIdentifier()
                {
                    return content.GetFieldIdentifier().FirstOrDefault()?
                              .ToString().Trim() == "const";
                }

                bool IsPartOfMethodDefinition()
                {
                    return declaration.GetDeclarationListParent()?.GetParent()?
                              .RuleName == "paran_group";
                }
            }
        }

        private static string[] GetUsing(ParseNode root)
        {
            List<string> result = new();
            foreach (ParseNode usingNode in root.GetHierarchy().Where(n => n.RuleType == "leaf" && n.RuleName == "identifier" && n.ToString() == "using"))
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

        private Dictionary<IType, CodePosition> GetTypes(ParseNode root, string[] usings, List<ParserMessage> messages)
        {
            Dictionary<IType, CodePosition> types = new Dictionary<IType, CodePosition>();
            foreach (ParseNode typeDeclaration in GetTypeDeclarations(root))
            {
                ParseNode content = GetDeclarationContentParent(typeDeclaration);
                if (!IsValidDeclaration(content, typeDeclaration))
                {
                    continue;
                }

                string name = GetName(typeDeclaration);
                string ns = GetNamespace(typeDeclaration);
                switch (typeDeclaration[1].RuleName)
                {
                    case "struct_decl":
                        CppStructure structure = new(ns, name, usings, content, messages,
                                                     typeDeclaration,
                                                     settingsProvider.Settings.AttributePrefix);
                        types.Add(structure,
                                  new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                        break;
                    case "class_decl":
                        CppClass cppClass = new(ns, name, usings, content, typeDeclaration[1], messages,
                                                settingsProvider.Settings.AttributePrefix);
                        types.Add(cppClass,
                                  new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                        break;
                    case "enum_decl":
                        if(IsAnonymousType(typeDeclaration))
                        { 
                            break; 
                        }
                        CppEnum cppEnum = new(ns, name, usings, content, messages, typeDeclaration[1],
                                              settingsProvider.Settings.AttributePrefix);
                        types.Add(cppEnum,
                                  new CodePosition(typeDeclaration.Position.line, typeDeclaration.Position.column));

                        break;
                }
            }

            return types;

            bool IsValidDeclaration(ParseNode content, ParseNode typeDeclaration)
            {
                return content?.Count >= 2 &&
                       content.Any(c => c.GetHierarchy().Contains(typeDeclaration)) &&
                       content.SkipWhile(c => !c.GetHierarchy().Contains(typeDeclaration))
                              .Skip(1).Any(c => c.GetHierarchy().Any(n => n.RuleName == "brace_group"));
            }

            bool IsAnonymousType(ParseNode typeDeclaration)
            {
                ParseNode leaf = typeDeclaration.GetHierarchy()
                                            .FirstOrDefault(n => n.RuleName == "identifier" || n.RuleName == "generic");
                if (leaf == null)
                {
                    return true;
                }
                return false;
            }
        }

        private static string GetNamespace(ParseNode typeDeclaration, bool skipFirst=true)
        {
            ParseNode declarationParent;
            string result = string.Empty;
            
            if (skipFirst)
            {
                typeDeclaration = GetDeclarationContentParent(typeDeclaration).GetParent();
            }
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
                                                                       .Where(c => c.RuleType == "choice" && c.RuleName == "node")
                                                                       .SelectMany(c => c.ChildrenSkipUnnamed())
                                                                       .FirstOrDefault(c => c.RuleType == "sequence" && c.RuleName == "type_decl");
                    ParseNode name = parentTypeDeclaration?.GetHierarchy()
                                                           .FirstOrDefault(n => n.RuleType == "leaf" && n.RuleName == "identifier");
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

        private static string GetName(ParseNode typeDeclaration)
        {
            ParseNode leaf = typeDeclaration.GetHierarchy()
                                            .FirstOrDefault(n => (n.RuleName == "identifier" && n.GetNamedParent().RuleName != "template_decl") 
                                                                 || n.RuleName == "generic");
            if (leaf == null)
            {
                return Guid.NewGuid().ToByteString();
            }

            return leaf.ToString();
        }

        private static ParseNode GetDeclarationContentParent(ParseNode current)
        {
            while (current != null && current.RuleName != "declaration_content")
            {
                current = current.GetParent();
            }

            return current;
        }

        private static bool HasTypeDeclarationParent(ParseNode current)
        {
            while (current != null && !IsTypeDeclaration())
            {
                current = GetDeclarationContentParent(current.GetParent());
            }

            return current != null;

            bool IsTypeDeclaration()
            {
                return current.ChildrenSkipUnnamed()
                              .Any(node => node.ChildrenSkipUnnamed()
                                               .Any(c => c.RuleName == "type_decl"));
            }
        }

        private static IEnumerable<ParseNode> GetTypeDeclarations(ParseNode current)
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

        private static ParseNode Identifier(ParseNode parent)
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

        private static string[] GetIncludes(ParseNode root)
        {
            List<string> result = new();
            foreach (ParseNode includeNode in root.GetHierarchy().Where(n => n.RuleType == "sequence" && n.RuleName == "pp_directive" && n.Any(c => c.ToString().Equals("include", StringComparison.OrdinalIgnoreCase))))
            {
                ParseNode include = includeNode.FirstOrDefault(n => n.RuleName == "until_eol");
                if (include != null)
                {
                    result.Add(include.ToString().Trim('\"'));
                }
            }

            return result.ToArray();
        }

        private static Dictionary<string, string> GetDefineStatements(ParseNode root)
        {
            Dictionary<string, string> directives = new();
            foreach (ParseNode defineNode in root.GetHierarchy().Where(n => n.RuleType == "sequence" && n.RuleName == "pp_directive" && n.Any(c => c.ToString().Equals("define", StringComparison.OrdinalIgnoreCase))))
            {
                string statement = defineNode.FirstOrDefault(n => n.RuleName == "until_eol")?.ToString() ?? string.Empty;
                Match match = StatementParser.Match(statement);
                string key = match.Groups["Key"].Value.Trim();
                if (match.Success && !directives.ContainsKey(key))
                {
                    directives.Add(match.Groups["Key"].Value, match.Groups["Value"].Value.Trim());
                }
            }

            return directives;
        }
    }
}
