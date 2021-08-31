#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace PlcNext.Common.Commands.CommandResults
{
    public class Path
    {
        public Path(string pathValue)
        {
            PathValue = pathValue;
        }

        [JsonProperty(PropertyName = "path")]
        public string PathValue { get; }
    }

    public class UncheckedPath : Path
    {
        public UncheckedPath(string pathValue, bool exists) : base(pathValue)
        {
            Exists = exists;
        }

        [JsonProperty(PropertyName = "exists")]
        public bool Exists { get; }
    }

    public class IncludePath : UncheckedPath
    {
        public IncludePath(string pathValue, bool exists, IEnumerable<TargetResult> targets) : base(pathValue, exists)
        {
            Targets = targets;
        }

        [JsonProperty(PropertyName = "targets")]
        public IEnumerable<TargetResult> Targets { get; }
    }

    public class SdkPath : Path
    {
        public SdkPath(string pathValue, IEnumerable<TargetResult> targets) : base(pathValue)
        {
            Targets = targets;
        }

        [JsonProperty(PropertyName = "targets")]
        public IEnumerable<TargetResult> Targets { get; }
    }
}