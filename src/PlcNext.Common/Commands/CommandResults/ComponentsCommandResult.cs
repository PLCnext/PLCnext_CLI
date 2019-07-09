#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PlcNext.Common.Commands.CommandResults
{
    public class ComponentsCommandResult
    {
        public ComponentsCommandResult(IEnumerable<ComponentResult> components)
        {
            Components = components;
        }
        
        public static ComponentsCommandResult Empty => new ComponentsCommandResult(Enumerable.Empty<ComponentResult>());

        [JsonProperty(PropertyName = "components")]
        public IEnumerable<ComponentResult> Components { get; }
    }

    public class ComponentResult
    {
        public ComponentResult(string name, string ns)
        {
            Name = name;
            Namespace = ns;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }
        
        [JsonProperty(PropertyName = "namespace")]
        public string Namespace { get; }
    }
}