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
using System.Text.RegularExpressions;
using PlcNext.Common.Templates.Description;

namespace PlcNext.Common.Templates
{
    internal static class TemplateExtensions
    {
        public static ICollection<string> TemplateNames(this TemplateDescription templateDescription,
                                                        ITemplateRepository templateRepository)
        {
            return templateDescription.IncludingBaseTemplates(templateRepository)
                                      .Select(t => t.name)
                                      .ToArray();
        }
        public static ICollection<TemplateDescription> IncludingBaseTemplates(this TemplateDescription templateDescription,
                                                        ITemplateRepository templateRepository)
        {
            List<TemplateDescription> templates = new List<TemplateDescription>();
            TemplateDescription template = templateDescription;
            while (template != null)
            {
                templates.Add(template);
                template = templateRepository.Template(template.basedOn);
            }

            return templates;
        }

        public static (bool success, string message, string newValue) Verify(
            this Field.valueRestriction restriction, string value)
        {
            if (!int.TryParse(restriction.length, out int length))
            {
                length = -1;
            }
            if (!int.TryParse(restriction.minlength, out int minLength))
            {
                minLength = 0;
            }
            if (!int.TryParse(restriction.maxlength, out int maxLength))
            {
                maxLength = -1;
            }

            if (!CheckLengths(value, length, minLength, maxLength,
                              out (bool success, string message, string newValue) result))
            {
                return result;
            }

            if (restriction.Items?.Any() == true &&
                !CheckRestriction(value, restriction.Items,
                                  restriction.ItemsElementName[0] == Field.ItemsChoiceType.Pattern,
                                  restriction.ignorecase,
                                  out result))
            {
                return result;
            }

            string newValue = result.newValue;
            switch (restriction.whitespace)
            {
                case Field.whitespace.Replace:
                    newValue = Regex.Replace(newValue, @"\s", " ");
                    break;
                case Field.whitespace.Collapse:
                    newValue = Regex.Replace(newValue, @"\s+", "");
                    break;
                default:
                    //preserve
                    break;
            }

            return (true, null, newValue);
        }

        public static (bool success, string message, string newValue) Verify(
            this Type.valueRestriction restriction, string value)
        {
            if (!int.TryParse(restriction.length, out int length))
            {
                length = -1;
            }
            if (!int.TryParse(restriction.minlength, out int minLength))
            {
                minLength = 0;
            }
            if (!int.TryParse(restriction.maxlength, out int maxLength))
            {
                maxLength = -1;
            }

            if (!CheckLengths(value, length, minLength, maxLength,
                              out (bool success, string message, string newValue) result))
            {
                return result;
            }

            if (restriction.Items?.Any() == true &&
                !CheckRestriction(value, restriction.Items,
                                  restriction.ItemsElementName[0] == Type.ItemsChoiceType.Pattern,
                                  restriction.ignorecase,
                                  out result))
            {
                return result;
            }

            string newValue = result.newValue;
            switch (restriction.whitespace)
            {
                case Type.whitespace.Replace:
                    newValue = Regex.Replace(newValue, @"\s", " ");
                    break;
                case Type.whitespace.Collapse:
                    newValue = Regex.Replace(newValue, @"\s+", "");
                    break;
                default:
                    //preserve
                    break;
            }

            return (true, null, newValue);
        }

        public static (bool success, string message, string newValue) Verify(
            this Description.valueRestriction restriction, string value)
        {
            if (!int.TryParse(restriction.length, out int length))
            {
                length = -1;
            }
            if (!int.TryParse(restriction.minlength, out int minLength))
            {
                minLength = 0;
            }
            if (!int.TryParse(restriction.maxlength, out int maxLength))
            {
                maxLength = -1;
            }

            if (!CheckLengths(value, length, minLength, maxLength,
                              out (bool success, string message, string newValue) result))
            {
                return result;
            }

            if (restriction.Items?.Any() == true &&
                !CheckRestriction(value, restriction.Items,
                                  restriction.ItemsElementName[0] == Description.ItemsChoiceType.Pattern,
                                  restriction.ignorecase,
                                  out result))
            {
                return result;
            }

            string newValue = result.newValue;
            switch (restriction.whitespace)
            {
                case Description.whitespace.Replace:
                    newValue = Regex.Replace(newValue, @"\s", " ");
                    break;
                case Description.whitespace.Collapse:
                    newValue = Regex.Replace(newValue, @"\s+", "");
                    break;
                default:
                    //preserve
                    break;
            }

            return (true, null, newValue);
        }

        private static bool CheckRestriction(string value, string[] restriction, bool isPattern, bool ignorecase,
                                             out (bool success, string message, string newValue) result)
        {
            if (isPattern)
            {
                bool matches = IsMatch(restriction[0]);
                result = matches
                             ? (true, null, value)
                             : (false, $"The value {value} does not match with the " +
                                       $"restriction pattern {restriction[0]}",
                                value);
                return matches;
            }

            string match = restriction.FirstOrDefault(r => r.Equals(value, ignorecase
                                                                               ? StringComparison.OrdinalIgnoreCase
                                                                               : StringComparison.Ordinal));
            string newValue = !string.IsNullOrEmpty(match) && ignorecase
                                  ? match
                                  : value;
            result = !string.IsNullOrEmpty(match)
                         ? (true, null, newValue)
                         : (false, $"The value {value} does not match with any of the following " +
                                   $"allowed values:{Environment.NewLine}" +
                                   $"{string.Join(Environment.NewLine, restriction)}",
                            value);
            return result.success;

            bool IsMatch(string pattern)
            {
                if (!pattern.StartsWith("^", StringComparison.Ordinal))
                {
                    pattern = $"^{pattern}";
                }

                if (!pattern.EndsWith("$", StringComparison.Ordinal))
                {
                    pattern += "$";
                }
                RegexOptions options = ignorecase ? RegexOptions.IgnoreCase : RegexOptions.None;
                Regex regex = new Regex(pattern, options);
                return regex.IsMatch(value);
            }
        }

        private static bool CheckLengths(string value, int length, int minLength, int maxLength,
                                         out (bool success, string message, string newValue) result)
        {
            if (length > -1 && value.Length != length)
            {
                result = (false,
                              $"The value {value} has the length {value.Length}, " +
                              $"but only a length of {length} was allowed for this value.",
                              value);
                return false;
            }

            if (value.Length < minLength)
            {
                result = (false,
                              $"The value {value} has the length {value.Length}, " +
                              $"but a minimal length of {minLength} was required for this value.",
                              value);
                return false;
            }

            if (maxLength > -1 && value.Length > maxLength)
            {
                result = (false,
                              $"The value {value} has the length {value.Length}, " +
                              $"but only a maximal length of {maxLength} was allowed for this value.",
                              value);
                return false;
            }
            result = (true, null, value);
            return true;
        }
        public static bool Verify(
            this Format.formatTemplate conversionTable, string raw, string converted)
        {
            return conversionTable.RegexConversion.Where(entry => entry.pattern.Equals(raw, entry.ignorecase
                                                                               ? StringComparison.OrdinalIgnoreCase
                                                                               : StringComparison.Ordinal)
                                                        && entry.converted.Equals(converted, entry.ignorecase
                                                                               ? StringComparison.OrdinalIgnoreCase
                                                                               : StringComparison.Ordinal))
                                           .Any();
        }

        private static readonly Regex VersionComparisonRegex = 
            new (@"(?<version1>(\d+.\d+(.\d+)?(.\d+)?)?)\s*(?<operator>(\<|\>|=)+)\s*(?<version2>\d+.\d+(.\d+)?(.\d+)?)", RegexOptions.Compiled);


        public static bool ResolveCondition(this string condition)
        {
            try
            {
                CalcEngine.CalcEngine calcEngine = new CalcEngine.CalcEngine();
                return (bool)calcEngine.Evaluate(condition);
            }
            catch (Exception ex)
            {
                Match match = VersionComparisonRegex.Match(condition);
                if (match.Success)
                {
                    if (Version.TryParse(match.Groups["version1"].Value, out Version version1)
                        && Version.TryParse(match.Groups["version2"].Value, out Version version2))
                    {
                        switch (match.Groups["operator"].Value)
                        {
                            case "<":
                                return version1 < version2;
                            case ">":
                                return version1 > version2;
                            case "=":
                            case "==":
                                return version1 == version2;
                            case "<=":
                                return version1 <= version2;
                            case ">=":
                                return version1 >= version2;
                            case "!=":
                                return version1 != version2;
                            default:
                                break;
                        }
                    }
                }
                throw new TemplateFileConditionParsingException(condition, ex);
            }
            throw new TemplateFileConditionParsingException(condition);
        }
    }
}
