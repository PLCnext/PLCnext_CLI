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
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.Settings;

namespace PlcNext.Common.MetaData
{
    internal class AttributeView : IAttributeView
    {
        private readonly string[] AllowedAttributes = new[]
        {
            "Input",
            "Output",
            "ReadOnly",
            "Retain",
            "Opc",
            "Ehmi",
            "ProfiCloud",
            "Archive"
        };
        private readonly Regex fieldCommentRegex;

        public AttributeView(ISettingsProvider settingsProvider)
        {
            fieldCommentRegex = new Regex(Regex.Escape(settingsProvider.Settings.AttributePrefix) +
                                          @"attributes\((?<attributes>.*)\)",
                                          RegexOptions.IgnoreCase);
        }

        public IEnumerable<string> Attributes(Port port)
        {
            (string Attributes, CodePosition Position) attributes = port.Attributes;

            return ParseAttributes(attributes);
        }

        private IEnumerable<string> ParseAttributes((string Attributes, CodePosition Position) attributes)
        {
            if (string.IsNullOrEmpty(attributes.Attributes))
            {
                return Enumerable.Empty<string>();
            }

            string[] splitted = attributes.Attributes.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            CheckUnkown();
            return splitted.Select(a => AllowedAttributes.First(other => other.Equals(a, StringComparison.OrdinalIgnoreCase)));

            void CheckUnkown()
            {
                string[] unkown = splitted
                                 .Where(a => !AllowedAttributes.Any(
                                                 other => other.Equals(a, StringComparison.OrdinalIgnoreCase)))
                                 .ToArray();

                if (unkown.Length == 1)
                {
                    throw CreateException(unkown[0]);
                }

                if (unkown.Length > 1)
                {
                    List<CodeSpecificException> exceptions = new List<CodeSpecificException>();
                    foreach (string a in unkown)
                    {
                        exceptions.Add(CreateException(a));
                    }

                    throw new AggregateException(exceptions);
                }

                CodeSpecificException CreateException(string attribute)
                {
                    int offset = attributes.Attributes.IndexOf(attribute, StringComparison.Ordinal);
                    return new CodeSpecificException("ARP0001", attributes.Position.Line,
                                                    attributes.Position.Column + offset);
                }
            }
        }

        public IEnumerable<string> Attributes(IField field)
        {
            (string Attributes, CodePosition Position) attributes = GetAttributes();

            return ParseAttributes(attributes);

            (string, CodePosition) GetAttributes()
            {
                (IComment comment, Match match) = field.Comments.Select(c => (c, fieldCommentRegex.Match(c.Content)))
                                                       .FirstOrDefault(m => m.Item2.Success);
                if (match != null)
                {
                    return (match.Groups["attributes"].Value,
                            new CodePosition(comment.Position.Line,
                                             comment.Position.Column + match.Groups["attributes"].Index));
                }

                return (null, null);
            }
        }
    }
}
