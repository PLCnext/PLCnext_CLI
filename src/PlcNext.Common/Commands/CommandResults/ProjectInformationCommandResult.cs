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
                                               IEnumerable<EntityResult> entities)
        {
            Name = name;
            Namespace = ns;
            Type = type;
            Targets = targets;
            Entities = entities;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "namespace")]
        public string Namespace { get; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "targets")]
        public IEnumerable<TargetResult> Targets { get; }

        [JsonProperty(PropertyName = "entities")]
        public IEnumerable<EntityResult> Entities { get; }

    }

    public class EntityResult
    {
        public EntityResult(string name, string ns, string type, IEnumerable<string> childEntities)
        {
            Name = name;
            Namespace = ns;
            Type = type;
            ChildEntities = childEntities;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "namespace")]
        public string Namespace { get; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; }

        [JsonProperty(PropertyName = "relatedEntity")]
        public IEnumerable<string> ChildEntities { get; }
    }
}
