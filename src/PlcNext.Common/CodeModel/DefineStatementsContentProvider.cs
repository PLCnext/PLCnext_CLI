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
using PlcNext.Common.Build;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.CodeModel
{
    internal class DefineStatementsContentProvider : PriorityContentProvider
    {
        private readonly ISdkRepository sdkRepository;
        private readonly ExecutionContext context;
        private static readonly Regex StringParser = new Regex(@"String\<(?<Macro>[\S]+)\>$", RegexOptions.Compiled);
        public DefineStatementsContentProvider(ISdkRepository sdkRepository, ExecutionContext context)
        {
            this.sdkRepository = sdkRepository;
            this.context = context;
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return key == EntityKeys.StringMacroReplaceKey && 
                   owner.Type == EntityKeys.FormatKey &&
                   owner.Root.HasValue<ICodeModel>();
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            Dictionary<string, string> macros = GetMarcros(owner);

            string value = owner.Owner.Value<string>();
            Match valueMatch = StringParser.Match(value);
            if (!valueMatch.Success)
            {
                return owner.Create(key, value);
            }
            
            string macro = ReplaceMarcos(valueMatch, macros);
            try
            {
                CalcEngine.CalcEngine calcEngine = new CalcEngine.CalcEngine();
                int parsed = (int) Math.Round((double) calcEngine.Evaluate(macro));
                value = StringParser.Replace(value, $"String<{parsed}>");
            }
            catch (Exception e)
            {
                context.WriteWarning($"Cannot resolve static string macro {valueMatch.Groups["Macro"].Value}");
                context.WriteVerbose($"Exception from CalcEngine: {e}");
            }
            
            return owner.Create(key, value);
        }

        private static string ReplaceMarcos(Match valueMatch, Dictionary<string, string> macros)
        {
            string macro = valueMatch.Groups["Macro"].Value;
            bool changed = true;
            while (changed)
            {
                string replacable = macros.Keys.FirstOrDefault(k => macro.Contains(k));
                changed = !string.IsNullOrEmpty(replacable);
                if (changed)
                {
                    macro = macro.Replace(replacable, macros[replacable]);
                }
            }
            return macro;
        }

        private Dictionary<string, string> GetMarcros(Entity owner)
        {
            ICodeModel codeModel = owner.Root.Value<ICodeModel>();
            ProjectEntity projectEntity = ProjectEntity.Decorate(owner.Root);
            SdkInformation sdk = projectEntity.ValidatedTargets.Where(t => t.HasValue<Target>())
                                              .Select(t => sdkRepository.GetSdk(t.Value<Target>()))
                                              .FirstOrDefault(s => s != null);
            Dictionary<string, string> macros = codeModel.DefineStatements;
            if (sdk != null)
            {
                foreach (CompilerMakro compilerMakro in sdk.CompilerInformation.Makros)
                {
                    if (macros.ContainsKey(compilerMakro.Name))
                    {
                        continue;
                    }

                    macros.Add(compilerMakro.Name, compilerMakro.Value);
                }
            }

            return macros;
        }
    }
}