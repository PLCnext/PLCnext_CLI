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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Build
{
    internal static class CodeModelExtensions
    {
        public static JObject GetProjectTarget(this JArray codeModel, string projectName, bool throwErrors = true)
        {
            JObject proj = codeModel.OfType<JObject>()
                                    .Where(o => o.ContainsKey("projects"))
                                    .SelectMany(o => o["projects"])
                                    .OfType<JObject>()
                                    .FirstOrDefault(o => o.ContainsKey("name") &&
                                                         o["name"].Value<string>() == projectName);
            if (proj == null)
            {
                if (throwErrors)
                {
                    throw new FormattableException($"The code model does not contain any project with the name '{projectName}'. " +
                                                   $"The code model contains the following data:{Environment.NewLine}" +
                                                   $"{codeModel.ToString(Formatting.Indented)}");
                }

                return null;
            }

            JObject projectTarget = proj.ContainsKey("targets")
                                        ? proj["targets"].OfType<JObject>()
                                                         .Where(o => o.ContainsKey("name"))
                                                         .FirstOrDefault(o => o["name"].Value<string>() == projectName)
                                        : null;
            if (projectTarget == null)
            {
                if (throwErrors)
                {
                    throw new FormattableException($"The code model of project '{projectName}' does not contain any target with the name '{projectName}'. " +
                                                   $"The project contains the following data:{Environment.NewLine}" +
                                                   $"{proj.ToString(Formatting.Indented)}");
                }

                return null;
            }

            return projectTarget;
        }
    }
}
