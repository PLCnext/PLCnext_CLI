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
    internal class CppClass : CppType, IClass
    {
        public CppClass(string ns, string name, string[] usings, ParseNode content, ParseNode classDeclaration,
                        List<ParserMessage> messages, string attributePrefix) : base(ns, name, usings, content, messages, classDeclaration, attributePrefix)
        {
        }
    }
}
