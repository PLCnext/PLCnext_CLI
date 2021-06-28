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
using System.Reflection;
using System.Text;
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

        public CppStructure(string ns, string name, IReadOnlyCollection<CppComment> comments, IEnumerable<CppField> fields, IEnumerable<CppDataType> baseTypes, string attributePrefix) : base(ns, name, comments, fields, baseTypes, attributePrefix)
        {
        }
    }
}
