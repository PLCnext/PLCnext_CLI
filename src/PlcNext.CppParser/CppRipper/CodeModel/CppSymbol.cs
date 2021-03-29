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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppSymbol : ISymbol
    {
        private CppSymbol(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static CppSymbol ParseSymbol(ParseNode[] content, CppSymbol lastSymbol)
        {
            string name = string.Join(string.Empty, content.Select(c => c.ToString()));
            string value = string.Empty;
            ParseNode valueSplitter = content.FirstOrDefault(c => c.GetHierarchy()
                                                                   .FirstOrDefault(n => n.RuleName == "symbol")
                                                                  ?.ToString().Contains("=") == true);
            if (valueSplitter != null && Array.IndexOf(content,valueSplitter) == 1)
            {
                name = content[0].ToString();
                value = string.Join(string.Empty, content.Skip(2).Select(c => c.ToString()));
            }
            else if (content.Length == 1)
            {
                name = content[0].ToString();
            }

            if (string.IsNullOrEmpty(value))
            {
                if (lastSymbol != null)
                {
                    if (long.TryParse(lastSymbol.Value, out long numberValue))
                    {
                        value = (numberValue + 1).ToString("D", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    value = "0";
                }
            }

            value = value.Trim().Trim('\'', '"');
            if (!long.TryParse(value, out _))
            {
                try
                {
                    long convertedValue = Convert.ToInt64(value, 16);
                    value = convertedValue.ToString("D", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    //do nothing
                }
            }
            return new CppSymbol(name, value);
        }

        public string Name { get; }
        public string Value { get; }
    }
}
