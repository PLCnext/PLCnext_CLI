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
    public class SettingCommandResult
    {
        public SettingCommandResult(object settings)
        {
            Settings = settings;
        }

        [JsonProperty(PropertyName = "setting")]
        public object Settings { get; }
    }
}