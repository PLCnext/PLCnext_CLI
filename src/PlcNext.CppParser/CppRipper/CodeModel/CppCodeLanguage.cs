#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using System.Reflection;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppCodeLanguage : ICodeLanguage
    {
        public string CombineNamespace(params string[] namespaceParts)
        {
            return string.Join("::", namespaceParts.Select(p => p.Trim(':')));
        }

        public string[] SplitNamespace(string ns)
        {
            return ns.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
