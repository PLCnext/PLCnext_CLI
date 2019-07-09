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
    public class IncludePathsCommandResult
    {
        public IncludePathsCommandResult(IEnumerable<Path> includePaths)
        {
            IncludePaths = includePaths;
        }
        
        public static IncludePathsCommandResult Empty => new IncludePathsCommandResult(Enumerable.Empty<Path>());

        [JsonProperty(PropertyName = "includePaths")]
        public IEnumerable<Path> IncludePaths { get; }
    }
}