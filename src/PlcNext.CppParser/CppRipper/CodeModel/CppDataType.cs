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
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppDataType : CppCodeEntity, IDataType
    {
        private readonly IEnumerable<string> potentialPrefixes;
        public IEnumerable<string> PotentialFullNames => potentialPrefixes.Select(p => $"{p}::{Name}")
                                                                          .Concat(new []{Name});
        
        public string Visibility { get; }

        public CppDataType(string name, string[] usings, string ns, string visibility = "public") : base(name, null)
        {
            IEnumerable<string> potentialNamespaces = GetNamespaces();
            potentialPrefixes = usings.Concat(potentialNamespaces);
            Visibility = visibility;

            IEnumerable<string> GetNamespaces()
            {
                string[] components = ns.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
                string current = null;
                foreach (string component in components)
                {
                    if (string.IsNullOrEmpty(current))
                    {
                        current = component;
                    }
                    else
                    {
                        current += $"::{component}";
                    }

                    yield return current;
                }
            }
        }
    }
}
