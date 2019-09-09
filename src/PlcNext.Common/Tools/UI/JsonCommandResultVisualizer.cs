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
    public class JsonCommandResultVisualizer : ICommandResultVisualizer
    {
        private readonly ExecutionContext executionContext;

        public JsonCommandResultVisualizer(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public void Visualize(object result, CommandArgs args)
        {
            if (result.GetType().GetCustomAttribute<CustomFormatDataStructureAttribute>() != null)
            {
                executionContext.WriteInformation(result.ToString());
                executionContext.WriteWarning($"This command is deprecated. Please use '{args.DeprecatedAlternative}' instead.");
            }
            else
            {
                JObject formattedObject = JObject.FromObject(result, new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                });
                if (args.Deprecated)
                {
                    formattedObject.Add("deprecated",new JValue($"This command is deprecated. Please use '{args.DeprecatedAlternative}' instead."));
                }
                executionContext.WriteInformation(formattedObject.ToString(Formatting.Indented));
            }
        }
    }
}