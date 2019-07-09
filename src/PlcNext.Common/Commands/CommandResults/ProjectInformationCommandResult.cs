#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Commands.CommandResults
{
    public class ProjectInformationCommandResult
    {
        public ProjectInformationCommandResult(string name, string ns, string type,
                                               IEnumerable<TargetResult> targets,
                                               IEnumerable<ComponentResult> components,
                                               IEnumerable<ProgramResult> programs)
        {
            Name = name;
            Namespace = ns;
            Type = type;
            Targets = targets;
            Components = components;
            Programs = programs;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "namespace")]
        public string Namespace { get; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "targets")]
        public IEnumerable<TargetResult> Targets { get; }

        [JsonProperty(PropertyName = "components")]
        public IEnumerable<ComponentResult> Components { get; }

        [JsonProperty(PropertyName = "programs")]
        public IEnumerable<ProgramResult> Programs { get; }

    }
}
