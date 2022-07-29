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
        protected string[] Usings { get; }
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

        internal bool IsTemplated { get; private set; }
        internal string[] TemplateArguments { get; private set; }

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

        protected void ReplaceTemplateArgumentsWithTypes(string[] templateIdentifier, string ns, string[] usings, string attributePrefix)
        {
            if (templateIdentifier.Length != TemplateArguments.Length)
            {
                throw new ArgumentException("Template arguments do not match number of given arguments.");
            }
            for (int i = 0; i < templateIdentifier.Length; i++)
            {
                IEnumerable<CppField> newFields = fields.Where(f => f.DataType.Name == TemplateArguments[i])
                                                        .Select(f => new CppField(f.Name,
                                                                        new CppDataType(templateIdentifier[i].Trim(), usings, ns),
                                                                        f.Comments,
                                                                        f.Multiplicity,
                                                                        attributePrefix)).ToArray();
                fields.RemoveAll(f => f.DataType.Name == TemplateArguments[i]);
                fields.AddRange(newFields);
            }
        }

        protected CppType(string ns, string name, IComment[] comments, IEnumerable<IDataType> baseTypes, string[] usings, string attributePrefix,
            string[] templateArguments, bool isTemplated, IEnumerable<CppField> fields)
            : base(name, attributePrefix)
        {
            Namespace = ns;
            Usings = usings;
            Comments = comments;
            this.baseTypes = new List<CppDataType>(baseTypes.Select(t => t is CppDataType ? t as CppDataType : null).Where(t => t != null));
            TemplateArguments = templateArguments;
            IsTemplated = isTemplated;
            this.fields.AddRange(fields);
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

            ParseTemplatedType();
            void ParseTemplatedType()
            {
                ParseNode typeDeclaration = content.GetHierarchy()
                                             .FirstOrDefault(c => c.RuleType == "sequence" && c.RuleName == "type_decl");

                IsTemplated = typeDeclaration.GetHierarchy().Any(c => c.RuleName == "template_decl");
                if (IsTemplated)
                {
                    ParseNode templateNode = typeDeclaration.GetHierarchy().First(c => c.RuleName == "template_decl");
                    TemplateArguments = templateNode.ChildrenSkipUnnamed().SkipWhile(c => c.RuleName != "TYPENAME")
                                                      .Where(c => c.RuleName == "identifier").Select(c => c.ToString()).ToArray();

                }
            }
        }
    }
}
