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
        public IEnumerable<IDataType> BaseTypes { get; }

        public string Namespace { get; }
        public string FullName => $"{Namespace}::{Name}";

        public IEnumerable<IField> Fields { get; }

        protected CppType(string ns, string name, IReadOnlyCollection<CppComment> comments, IEnumerable<CppField> fields,
                          IEnumerable<CppDataType> baseTypes, string attributePrefix)
            : base(name, attributePrefix)
        {
            Namespace = ns;
            Comments = comments;
            Fields = fields;
            BaseTypes = baseTypes;
            foreach (CppField field in Fields.Cast<CppField>())
            {
                field.RegisterContainingType(this);
            }
        }
    }
}
