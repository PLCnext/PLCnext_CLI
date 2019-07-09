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
    public class ProjectNamespaceCommandResult
    {
        public ProjectNamespaceCommandResult(string ns)
        {
            Namespace = ns;
        }

        [JsonProperty(PropertyName = "namespace")]
        public string Namespace { get; }
    }
}
