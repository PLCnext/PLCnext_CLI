#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Commands;

namespace PlcNext.Common.Tools.UI
{
    internal class JsonCommandResultVisualizer : ICommandResultVisualizer
    {
        private readonly ExecutionContext executionContext;

        public JsonCommandResultVisualizer(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public void Visualize(object result, CommandArgs args, string errorMessage)
        {
            if (result?.GetType().GetCustomAttribute<CustomFormatDataStructureAttribute>() != null)
            {
                executionContext.WriteInformation(result.ToString());
                executionContext.WriteWarning($"This command is deprecated. Please use '{args.DeprecatedAlternative}' instead.");
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    executionContext.WriteError(errorMessage);
                }
            }
            else
            {
                JObject formattedObject = result != null
                                              ? JObject.FromObject(result, new JsonSerializer
                                              {
                                                  NullValueHandling = NullValueHandling.Include,
                                                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                  StringEscapeHandling = StringEscapeHandling.EscapeHtml
                                              })
                                              : new JObject();
                if (args.Deprecated)
                {
                    formattedObject.Add("deprecated",new JValue($"This command is deprecated. Please use '{args.DeprecatedAlternative}' instead."));
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    formattedObject.Add("error", new JValue(errorMessage));
                }
                executionContext.WriteInformation(formattedObject.ToString(Formatting.Indented));
            }
        }
    }
}