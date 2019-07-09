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
    public class ProgramsCommandResult
    {
        public ProgramsCommandResult(IEnumerable<ProgramResult> programs)
        {
            Programs = programs;
        }

        public static ProgramsCommandResult Empty => new ProgramsCommandResult(Enumerable.Empty<ProgramResult>());
        
        [JsonProperty(PropertyName = "programs")]
        public IEnumerable<ProgramResult> Programs { get; }
    }

    public class ProgramResult
    {
        public ProgramResult(string programName, string programNamespace, string componentName, string componentNamespace)
        {
            ProgramName = programName;
            ProgramNamespace = programNamespace;
            ComponentName = componentName;
            ComponentNamespace = componentNamespace;
        }

        [JsonProperty(PropertyName = "name")]
        public string ProgramName { get; }
        
        [JsonProperty(PropertyName = "namespace")]
        public string ProgramNamespace { get; }
        
        [JsonProperty(PropertyName = "componentName")]
        public string ComponentName { get; }
        
        [JsonProperty(PropertyName = "componentNamespace")]
        public string ComponentNamespace { get; }
    }
}