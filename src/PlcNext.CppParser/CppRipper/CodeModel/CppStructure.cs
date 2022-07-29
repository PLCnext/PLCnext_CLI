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

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppStructure : CppType, IStructure
    {
        public CppStructure(string ns, string name, string[] usings, ParseNode content, List<ParserMessage> messages,
                            ParseNode structureDeclaration, string attributePrefix) : base(ns, name, usings, content, messages, structureDeclaration, attributePrefix)
        {
        }

        internal CppStructure(string ns, string name, IComment[] comments, IEnumerable<IDataType> baseTypes, string[] usings, string attributePrefix,
            string[] templateArguments, bool isTemplated, IEnumerable<CppField> fields)
            : base(ns, name, comments, baseTypes, usings, attributePrefix, templateArguments, isTemplated, fields)
        {
        }

        internal CppTemplatedStructure CreateTemplatedStructure(string name, string[] templateIdentifier)
        {
            return new CppTemplatedStructure(Namespace, name, Comments.ToArray(), BaseTypes, Usings, AttributePrefix, templateIdentifier,
                TemplateArguments, IsTemplated, Fields.Select(f => f as CppField));
        }
    }
}
