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
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel.Parser
{
    internal static class GeneralParser
    {
        public static CodePosition GetCodePosition(this ParseNode node)
        {
            return new CodePosition(node.Position.line, node.Position.column);
        }
        
        public static IEnumerable<string> IterateNamespacePermutations(this string ns)
        {
            string[] split = ns.Split(new []{"::"},StringSplitOptions.RemoveEmptyEntries);
            string current = string.Empty;
            yield return current;
            foreach (string part in split)
            {
                current = string.IsNullOrEmpty(current) ? part : $"{current}::" + part;
                yield return current;
            }
        }

        internal static ParseNode GetDeclarationContentParent(this ParseNode current)
        {
            while (current != null &&
                   current.RuleType != "plus" &&
                   current.RuleName != "declaration_content")
            {
                current = current.GetParent();
            }

            return current;
        }

        internal static ParseNode GetDeclarationListParent(this ParseNode current)
        {
            while (current != null &&
                   current.RuleType != "recursive" &&
                   current.RuleName != "declaration_list")
            {
                current = current.GetParent();
            }

            return current;
        }
    }
}