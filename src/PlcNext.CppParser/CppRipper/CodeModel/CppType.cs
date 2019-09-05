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
    internal abstract class CppType : CppCodeEntity, IType
    {
        private readonly List<CppDataType> baseTypes = new List<CppDataType>();
        public IEnumerable<IDataType> BaseTypes => baseTypes;

        public string Namespace { get; }
        public string FullName => $"{Namespace}::{Name}";
        protected ParseNode GetDeclarationList(ParseNode content, ParseNode typeDeclaration)
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


        private readonly List<IField> fields = new List<IField>();

        public virtual IEnumerable<IField> Fields => fields;
        
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

            IEnumerable<string> baseTypeNames = typeDeclaration.GetBaseTypes();
            baseTypes.AddRange(baseTypeNames.Select(n => new CppDataType(n, usings, ns)));
        }
    }
}
