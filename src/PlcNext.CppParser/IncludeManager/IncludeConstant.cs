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
using Newtonsoft.Json;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.IncludeManager
{
    internal sealed class IncludeConstant : IConstant
    {
        public static IncludeConstant Parse(IConstant arg)
        {
            return new IncludeConstant
            {
                Name = arg.Name,
                Namespace = arg.Namespace,
                AccessibleNamespaces = arg.AccessibleNamespaces,
                Value = arg.Value,
                FullName = arg.FullName
            };
        }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Value { get; set; }
        public string FullName { get; set; }
        public IEnumerable<string> AccessibleNamespaces { get; set; }
        
        [JsonIgnore]
        public IReadOnlyCollection<IComment> Comments { get; } = Array.Empty<IComment>();
        [JsonIgnore]
        public IReadOnlyCollection<IAttribute> Attributes { get; } = Array.Empty<IAttribute>();
    }
}