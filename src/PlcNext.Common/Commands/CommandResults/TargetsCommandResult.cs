#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PlcNext.Common.Commands.CommandResults
{
    public class TargetsCommandResult
    {
        public TargetsCommandResult(IEnumerable<TargetResult> targets)
        {
            Targets = targets;
        }

        [JsonProperty(PropertyName = "targets")]
        public IEnumerable<TargetResult> Targets { get; }
    }

    public class TargetResult
    {
        public TargetResult(string name, string version, string longVersion, string shortVersion, bool? available = null)
        {
            Name = name;
            Version = version;
            LongVersion = longVersion;
            ShortVersion = shortVersion;
            Available = available;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }
        
        [JsonProperty(PropertyName = "version")]
        public string Version { get; }
        
        [JsonProperty(PropertyName = "longVersion")]
        public string LongVersion { get; }
        
        [JsonProperty(PropertyName = "shortVersion")]
        public string ShortVersion { get; }
        
        [JsonProperty(PropertyName = "available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Available { get; }
    }
}