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

namespace PlcNext.Common.Commands.CommandResults
{
    public class NameCommandResult
    {
        public NameCommandResult(string name)
        {
            Name = name;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; }
    }
}