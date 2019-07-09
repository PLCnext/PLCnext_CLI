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

namespace PlcNext.Common.Tools.UI
{
    public class JsonCommandResultVisualizer : ICommandResultVisualizer
    {
        private readonly ExecutionContext executionContext;

        public JsonCommandResultVisualizer(ExecutionContext executionContext)
        {
            this.executionContext = executionContext;
        }

        public void Visualize(object result)
        {
            if (result.GetType().GetCustomAttribute<CustomFormatDataStructureAttribute>() != null)
            {
                executionContext.WriteInformation(result.ToString());
            }
            else
            {
                executionContext.WriteInformation(JsonConvert.SerializeObject(
                                                      result, Formatting.Indented,
                                                      new JsonSerializerSettings
                                                      {
                                                          NullValueHandling = NullValueHandling.Include,
                                                          ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                          StringEscapeHandling = StringEscapeHandling.EscapeHtml
                                                      }));
            }
        }
    }
}