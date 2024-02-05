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
                                               IEnumerable<EntityResult> entities,
                                               IEnumerable<UncheckedPath> includePaths,
                                               IEnumerable<Path> externalLibraries, bool generateNamespaces,
                                               string cSharpProjectPath)
        {
            Name = name;
            Namespace = ns;
            Type = type;
            Targets = targets;
            Entities = entities;
            IncludePaths = includePaths;
            ExternalLibraries = externalLibraries;
            GenerateNamespaces = generateNamespaces;
            CSharpProjectPath = cSharpProjectPath;
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

        [JsonProperty(PropertyName = "includePaths")]
        public IEnumerable<Path> IncludePaths { get; }

        [JsonProperty(PropertyName ="externalLibraries")]
        public IEnumerable<Path> ExternalLibraries { get; }

        [JsonProperty(PropertyName = "generateNamespaces")]
        public bool GenerateNamespaces { get; }

        [JsonProperty(PropertyName = "cSharpProjectPath", NullValueHandling = NullValueHandling.Ignore, Required = Required.Default)]
        public string CSharpProjectPath { get; }
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
