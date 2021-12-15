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
using System.Text.RegularExpressions;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public abstract class CppCodeEntity : ICodeEntity
    {
        private bool reparseAttributes;
        private IComment[] comments = Array.Empty<IComment>();
        private IAttribute[] attributes = Array.Empty<IAttribute>();
        private readonly Regex attributeRegex;

        protected CppCodeEntity(string name, string attributePrefix)
        {
            attributeRegex = new Regex(Regex.Escape(attributePrefix??string.Empty) + @"(?<name>\w+)(?:\((?<value>.*)\))?",
                                            RegexOptions.Compiled);
            Name = name;
        }

        public string Name { get; protected set; }

        public IReadOnlyCollection<IComment> Comments
        {
            get => comments;
            protected set
            {
                comments = value.ToArray();
                reparseAttributes = true;
            }
        }

        public IReadOnlyCollection<IAttribute> Attributes
        {
            get
            {
                if (reparseAttributes)
                {
                    List<IAttribute> result = new List<IAttribute>();
                    foreach (IComment comment in Comments)
                    {
                        string content = WithoutSpanComments(comment);
                        Match attributeMatch = attributeRegex.Match(content);
                        while (attributeMatch.Success)
                        {
                            int lineOffset = content.Substring(0, attributeMatch.Index - 1)
                                                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                                    .Length - 1;
                            int columnOffset = Math.Max(content.Substring(0, attributeMatch.Index - 1)
                                                               .LastIndexOf(Environment.NewLine, StringComparison.Ordinal),
                                                        0);
                            if (columnOffset > 0)
                            {
                                columnOffset += Environment.NewLine.Length;
                                columnOffset -= content.Substring(columnOffset, attributeMatch.Index - columnOffset)
                                                       .Count(c => c == '\t') * 4;
                            }

                            CodePosition position = new CodePosition(comment.Position.Line + lineOffset,
                                                                     columnOffset > 0
                                                                         ? attributeMatch.Groups["value"].Index -
                                                                           columnOffset
                                                                         : comment.Position.Column +
                                                                           attributeMatch.Groups["value"].Index);
                            result.Add(attributeMatch.Groups["value"].Success
                                           ? new CppAttribute(attributeMatch.Groups["name"].Value,
                                                              attributeMatch.Groups["value"].Value,
                                                              position)
                                           : new CppAttribute(attributeMatch.Groups["name"].Value, position));
                            attributeMatch = attributeMatch.NextMatch();
                        }
                    }

                    attributes = result.ToArray();
                }
                return attributes;

                string WithoutSpanComments(IComment comment)
                {
                    string content = comment.Content;
                    IEnumerable<(int, int)> spanRegions = GetSpanRegions(content);
                    int lengthReduction = 0;
                    foreach ((int start, int end) in spanRegions)
                    {
                        int length = end - start;
                        content = content.Remove(start - lengthReduction, length);
                        lengthReduction += length;
                    }

                    return content;
                }

                IEnumerable<(int, int)> GetSpanRegions(string s)
                {
                    List<(int, int)> valueTuples = new List<(int, int)>();
                    string tag = string.Empty;
                    int start = -1;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == '/')
                        {
                            if (tag == "*")
                            {
                                if (start >= 0)
                                {
                                    valueTuples.Add((start, i + 1));
                                    start = -1;
                                    tag = string.Empty;
                                }
                            }
                            else if (string.IsNullOrEmpty(tag))
                            {
                                tag = "/";
                            }
                            else
                            {
                                tag = string.Empty;
                            }
                        }
                        else if (s[i] == '*')
                        {
                            if (tag == "/")
                            {
                                if (start < 0)
                                {
                                    start = i - 1;
                                    tag = string.Empty;
                                }
                            }
                            else if (string.IsNullOrEmpty(tag))
                            {
                                tag = "*";
                            }
                            else
                            {
                                tag = string.Empty;
                            }
                        }
                        else
                        {
                            tag = string.Empty;
                        }
                    }

                    return valueTuples;
                }
            }
        }
    }
}
