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
using System.Reflection;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal abstract class CppType : CppCodeEntity, IType
    {
        private readonly List<CppDataType> baseTypes = new List<CppDataType>();
        public IEnumerable<IDataType> BaseTypes => baseTypes;

        public string Namespace { get; }
        private string[] Usings { get; }
        public string FullName => $"{Namespace}::{Name}";
        protected static ParseNode GetDeclarationList(ParseNode content, ParseNode typeDeclaration)
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


        private readonly List<CppField> fields = new List<CppField>();

        public virtual IEnumerable<IField> Fields => fields;
        public IEnumerable<string> AccessibleNamespaces => Namespace.IterateNamespacePermutations().Concat(Usings);

        private void ParseFields(string ns, string[] usings, ParseNode content, List<ParserMessage> messages,
                                 ParseNode typeDeclaration, string attributePrefix)
        {
            ParseNode list = GetDeclarationList(content, typeDeclaration);
            if (list != null)
            {
                foreach (ParseNode declaration in list.ChildrenSkipUnnamed().Where(n => n.RuleName == "declaration"))
                {
                    fields.AddRange(CppField.Parse(declaration, usings, $"{ns}::{Name}", messages, attributePrefix, this));
                }
            }
        }

        protected CppType(string ns, string name, string[] usings, ParseNode content, List<ParserMessage> messages,
                          ParseNode typeDeclaration, string attributePrefix, bool parseFields = true) : base(name, attributePrefix)
        {
            Namespace = ns;
            Usings = usings;
            if (parseFields)
            {
                ParseFields(ns, usings, content, messages, typeDeclaration, attributePrefix);
            }

            Comments = GetComments();

            IComment[] GetComments()
            {
                //ParseNode content = declaration.ChildrenSkipUnnamed().First(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
                return content.ChildrenSkipUnnamed()
                              .Where(IsComment)
                              .Where(c => !string.IsNullOrEmpty(c.ToString()))
                              .Select(CppComment.Parse)
                              .ToArray();

                bool IsComment(ParseNode node)
                {
                    return node.RuleType == "sequence" && node.RuleName == "comment_set";
                }
            }

            //content first is type dec // children is comment

            IEnumerable<(string, string)> baseTypeNames = typeDeclaration.GetBaseTypes();
            baseTypes.AddRange(baseTypeNames.Select(n => new CppDataType(n.Item1, usings, ns, n.Item2)));
        }
    }
}
