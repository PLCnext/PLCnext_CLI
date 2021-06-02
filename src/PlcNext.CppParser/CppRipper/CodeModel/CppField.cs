#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppField : CppCodeEntity, IField
    {
        public static readonly Regex EqualsMatch = new Regex("^\\s?=\\s?$", RegexOptions.Compiled);
        public CppField(string name, IDataType dataType, IComment[] comments, int[] multiplicity, string attributePrefix, IType containingType = null) : base(name, attributePrefix)
        {
            DataType = dataType;
            Comments = comments;
            Multiplicity = multiplicity;
            ContainingType = containingType;
        }

        public IDataType DataType { get; }
        public IReadOnlyCollection<int> Multiplicity { get; }
        public IType ContainingType { get; private set; }

        public void RegisterContainingType(CppType cppType)
        {
            ContainingType = cppType;
        }
    }
}
