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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates.Format;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.Templates
{
    internal class FormatTemplateContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateResolver resolver;

        public FormatTemplateContentProvider(ITemplateRepository templateRepository, ITemplateResolver resolver)
        {
            this.templateRepository = templateRepository;
            this.resolver = resolver;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.FormatKey ||
                   owner.Values<formatTemplate>().Any(t => t.name.Equals(key, StringComparison.OrdinalIgnoreCase)) ||
                   owner.Value<Match>() != null ||
                   (key == EntityKeys.ValueKey &&
                    owner.Value<FormatValueAccess>() != null);
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            if (key == EntityKeys.FormatKey)
            {
                return owner.Create(key, templateRepository.FormatTemplates.Cast<object>().ToArray());
            }

            if (owner.Value<Match>() != null)
            {
                Match match = owner.Value<Match>();
                if (int.TryParse(key, out int index))
                {
                    if (match.Groups.Count > index)
                    {
                        return owner.Create(key, match.Groups[index].Value);
                    }
                }
                else if (owner.Value<Regex>()?.GetGroupNames()
                              .Contains(key,StringComparer.OrdinalIgnoreCase) 
                         == true)
                {
                    string realKey = owner.Value<Regex>()?.GetGroupNames()
                                          .First(n => n.Equals(key, StringComparison.OrdinalIgnoreCase));
                    return owner.Create(key, match.Groups[realKey].Value);
                }
                throw new ContentProviderException(key, owner);
            }

            if (key == EntityKeys.ValueKey)
            {
                return owner.Create(key, owner.Values<FormatValueAccess>()
                                              .OrderByDescending(a => a.Created)
                                              .First().Value);
            }

            IEnumerable<formatTemplate> templates = owner.Values<formatTemplate>();
            formatTemplate template = templates.FirstOrDefault(t => t.name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (template != null)
            {
                return owner.Create(key, new Func<string>(() => FormatValue(owner.Owner, template)));
            }
            throw new ContentProviderException(key, owner);
        }

        private string FormatValue(Entity valueProvider, formatTemplate template)
        {
            Entity original = CheckTarget();
            IEnumerable<string> values = GetValues();
            IEnumerable<string> formattedValues = values.Select(Format);
            string formattedValue = string.Join(template.seperator, formattedValues);
            return ApplyPrefixSuffix(formattedValue, true);

            Entity CheckTarget()
            {
                Entity result = valueProvider;
                while (result.Owner != null && result.Owner.Type == EntityKeys.FormatKey)
                {
                    result = result.Owner.Owner;
                }

                if (!string.IsNullOrEmpty(template.target) &&
                    !template.target.Equals(result.Type, StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatTargetMismatchException(template, result.Type);
                }

                return result;
            }

            IEnumerable<string> GetValues()
            {
                return template.multiplicity == multiplicity.OneOrMore
                           ? (template.Split?.Any() == true
                                  ? valueProvider.Value<string>().Split(template.Split.Select(Resolve).ToArray(), StringSplitOptions.None)
                                  : valueProvider.Select(e => e.Value<string>()))
                           : new[] {valueProvider.Value<string>()};

                string Resolve(string split)
                {
                    return resolver.Resolve(split, original);
                }
            }

            string ApplyPrefixSuffix(string mainString, bool onlyAggregated)
            {
                prefixSuffixFormat perValuePrefix = template.Prefix?.FirstOrDefault(p => p.onlyaggregated == onlyAggregated);
                string prefix = perValuePrefix != null
                                    ? resolver.Resolve(perValuePrefix.value, valueProvider.Empty)
                                    : string.Empty;
                prefixSuffixFormat perValueSuffix = template.Suffix?.FirstOrDefault(p => p.onlyaggregated == onlyAggregated);
                string suffix = perValueSuffix != null
                                    ? resolver.Resolve(perValueSuffix.value, valueProvider.Empty)
                                    : string.Empty;
                return prefix + mainString + suffix;
            }

            string Format(string value)
            {
                string result = ApplyPatterns();
                return ApplyPrefixSuffix(result, false);

                string ApplyPatterns()
                {
                    foreach (conditionalConversionFormat format in template.ConditionalConversion ?? Enumerable.Empty<conditionalConversionFormat>())
                    {
                        if (bool.TryParse(resolver.Resolve(format.condition, original), out bool applies) &&
                            applies)
                        {
                            using (original.AddTemporaryDataSource(new FormatValueAccess(value)))
                            {
                                return resolver.Resolve(format.template, original);
                            }
                        }
                    }

                    foreach (regexConversionFormat format in template.RegexConversion ?? Enumerable.Empty<regexConversionFormat>())
                    {
                        Match match = CreateMatch(format.pattern, format.ignorecase, out Regex regex);
                        if (match.Success)
                        {
                            return resolver.Resolve(format.converted, valueProvider.Create("regex", match, regex));
                        }
                    }

                    foreach (templateConversionFormat format in template.TemplateConversion ?? Enumerable.Empty<templateConversionFormat>())
                    {
                        Match match = CreateMatch(format.pattern, format.ignorecase, out Regex _);
                        if (match.Success)
                        {
                            using (original.AddTemporaryDataSource(new FormatValueAccess(value)))
                            {
                                return resolver.Resolve(format.template, original);
                            }
                        }
                    }

                    return value;

                    Match CreateMatch(string pattern, bool ignorecase, out Regex regex)
                    {
                        if (!pattern.StartsWith("^"))
                        {
                            pattern = $"^{pattern}";
                        }

                        if (!pattern.EndsWith("$"))
                        {
                            pattern += "$";
                        }
                        RegexOptions options = ignorecase ? RegexOptions.IgnoreCase : RegexOptions.None;
                        regex = new Regex(pattern, options);
                        return regex.Match(value);
                    }
                }
            }
        }

        private class FormatValueAccess
        {
            public FormatValueAccess(string value)
            {
                Value = value;
            }

            public string Value { get; }
            public DateTime Created { get; } = DateTime.Now;
        }
    }
}
