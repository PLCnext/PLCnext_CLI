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
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal sealed class CppAttribute : IAttribute
    {
        public CppAttribute(string name, CodePosition position) : this(name, null, position)
        {
        }

        public CppAttribute(string name, string value, CodePosition position)
        {
            Name = name;
            Position = position;
            Values = value != null ? new[] { value } : Array.Empty<string>();
        }

        public CodePosition Position { get; }
        public string Name { get; }
        public IEnumerable<string> Values { get; }
        public IDictionary<string, string> NamedValues { get; } = new Dictionary<string, string>();
    }
}
