#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class CppTemplatedStructure : CppStructure
    {
        public CppTemplatedStructure(string ns, string name, IComment[] comments, IEnumerable<IDataType> baseTypes, string[] usings, 
            string attributePrefix, string[] templateIdentifier, string[] templateArguments, bool isTemplated, IEnumerable<CppField> fields) 
            : base(ns, name, comments, baseTypes, usings, attributePrefix, templateArguments, isTemplated, fields)
        {
            ReplaceTemplateArgumentsWithTypes(templateIdentifier, ns, usings, attributePrefix);
        }
    }
}
