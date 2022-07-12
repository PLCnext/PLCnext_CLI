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
using System.Text.RegularExpressions;
using PlcNext.Common.Build;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.CodeModel
{
    internal class CodeConstantsContentProvider : PriorityContentProvider
    {
        private readonly ISdkRepository sdkRepository;
        private readonly ExecutionContext context;
        private static readonly Regex StringParser = new Regex(@"String\<(?<Macro>[\S]+)\>$", RegexOptions.Compiled);
        public CodeConstantsContentProvider(ISdkRepository sdkRepository, ExecutionContext context)
        {
            this.sdkRepository = sdkRepository;
            this.context = context;
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key is EntityKeys.StringConstantReplaceKey or EntityKeys.MultiplicityConstantReplaceKey && 
                   owner.Type == EntityKeys.FormatKey &&
                   owner.Root.HasValue<ICodeModel>();
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            switch (key)
            {
                case EntityKeys.StringConstantReplaceKey:
                    return ResolveStaticString(owner, key);
                case EntityKeys.MultiplicityConstantReplaceKey:
                    return ResolveMultiplicity(owner, key);
                default:
                    throw new ContentProviderException(key, owner);
            }
        }

        private Entity ResolveMultiplicity(Entity owner, string key)
        {
            IEnumerable<Entity> parsed = owner.Owner.Select(c => c.Value<string>())
                                              .Select(multiplicity => new { origin = multiplicity, parsed = ParseString(owner, multiplicity) })
                                              .Select(a => a.parsed.HasValue ? a.parsed.ToString() : a.origin)
                                              .Select(parsed => owner.Create(key, parsed));
            return owner.Create(key, parsed);
        }

        private Entity ResolveStaticString(Entity owner, string key)
        {
            string value = owner.Owner.Value<string>();
            Match valueMatch = StringParser.Match(value);
            if (!valueMatch.Success)
            {
                return owner.Create(key, value);
            }

            string replaceable = valueMatch.Groups["Macro"].Value;

            int? parsed = ParseString(owner, replaceable);

            if (parsed != null)
            {
                value = StringParser.Replace(value, $"String<{parsed}>");
            }

            return owner.Create(key, value);
        }

        private int? ParseString(Entity owner, string replaceable)
        {
            Dictionary<string, string> macros = GetDefineConstants(owner);
            (string macro, bool changed) = ReplaceWithDefineConstants(replaceable, macros);
            if (!changed)
            {
                macro = ReplaceWithConstants(replaceable, owner);
            }

            int? parsed = null;
            try
            {
                object result = Calculator.Evaluate(macro);
                if (result is double)
                {
                    parsed = (int)Math.Round((double)result);
                }
                parsed = (int)result;
            }
            catch (Exception e)
            {
                context.WriteWarning($"Cannot resolve static string macro {replaceable}");
                context.WriteVerbose($"Exception from CalcEngine: {e}");
            }

            return parsed;
        }

        private static string ReplaceWithConstants(string replaceable, Entity owner)
        {
            ICodeModel codeModel = owner.Root.Value<ICodeModel>();
            IType containingType = CodeEntity.Decorate(owner).FindContainingTypeInHierarchy();
            return ReplaceConstants(replaceable, containingType.AccessibleNamespaces, codeModel);
        }

        private record ConstantReplacement(string PartialName, string Replacement, IConstant Constant)
        {
            public virtual bool Equals(ConstantReplacement other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return PartialName == other.PartialName;
            }

            public override int GetHashCode()
            {
                return (PartialName != null ? PartialName.GetHashCode() : 0);
            }
        }

        private record TargetedReplacement(string Replacement, IConstant Constant, int StartIndex,
                                           int Length)
        {
            public bool Overlaps(TargetedReplacement other)
            {
                return StartIndex <= other.StartIndex && StartIndex+Length >= other.StartIndex;
            }
        };

        private static string ReplaceConstants(string replaceable, IEnumerable<string> accessibleNamespaces, ICodeModel codeModel)
        {
            IEnumerable<ConstantReplacement> accessibleConstants = GetAccessibleConstants();
            TargetedReplacement[] targetedReplacements = GetReplacementTargets();
            IEnumerable<TargetedReplacement> filteredReplacements = FilterReplacements();

            return filteredReplacements.Reverse()
                                       .Aggregate(replaceable, (current, replacement) => current.Remove(replacement.StartIndex, replacement.Length)
                                                     .Insert(replacement.StartIndex, ReplaceConstants(replacement.Replacement, 
                                                                 replacement.Constant.AccessibleNamespaces, 
                                                                 codeModel)));

            IEnumerable<ConstantReplacement> GetAccessibleConstants()
                => codeModel.FindAccessibleConstants(accessibleNamespaces)
                            .Select(a => new ConstantReplacement(
                                        a.Value.Length > 0 
                                                    ? a.Key.FullName.Substring(a.Value.Length + 2)
                                                    : a.Key.FullName,
                                        a.Key.Value,
                                        a.Key))
                            .Distinct();

            TargetedReplacement[] GetReplacementTargets()
            {
                return accessibleConstants.Where(c => replaceable.Contains(c.PartialName))
                                          .Select(c => new TargetedReplacement(c.Replacement,
                                                      c.Constant,
                                                      replaceable.IndexOf(c.PartialName, StringComparison.Ordinal),
                                                      c.PartialName.Length))
                                          .ToArray();
            }

            IEnumerable<TargetedReplacement> FilterReplacements()
            {
                TargetedReplacement lastReplacement = null;
                IOrderedEnumerable<TargetedReplacement> orderedReplacements = targetedReplacements.OrderBy(r => r.StartIndex)
                                                                                                  .ThenByDescending(r => r.Length);
                foreach (TargetedReplacement targetedReplacement in orderedReplacements)
                {
                    if (lastReplacement != null && 
                        lastReplacement.Overlaps(targetedReplacement))
                    {
                        continue;
                    }

                    yield return targetedReplacement;
                    lastReplacement = targetedReplacement;
                }
            }
        }

        private static (string, bool) ReplaceWithDefineConstants(string macro, Dictionary<string, string> macros)
        {
            bool changed = true;
            bool replaced = false;
            while (changed)
            {
                string replacable = macros.Keys.FirstOrDefault(k => macro.Contains(k));
                changed = !string.IsNullOrEmpty(replacable);
                if (changed)
                {
                    macro = macro.Replace(replacable, macros[replacable]);
                    replaced = true;
                }
            }
            return (macro, replaced);
        }

        private Dictionary<string, string> GetDefineConstants(Entity owner)
        {
            ICodeModel codeModel = owner.Root.Value<ICodeModel>();
            ProjectEntity projectEntity = ProjectEntity.Decorate(owner.Root);
            SdkInformation sdk = projectEntity.ValidatedTargets.Where(t => t.HasValue<Target>())
                                              .Select(t => sdkRepository.GetSdk(t.Value<Target>()))
                                              .FirstOrDefault(s => s != null);
            Dictionary<string, string> constants = codeModel.DefineStatements;
            if (sdk != null)
            {
                foreach (CompilerMakro compilerMakro in sdk.CompilerInformation.Makros)
                {
                    if (constants.ContainsKey(compilerMakro.Name))
                    {
                        continue;
                    }

                    constants.Add(compilerMakro.Name, compilerMakro.Value);
                }
            }

            return constants;
        }
    }
}