#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Commands
{
    public class BuildCommandArgs : CommandArgs
    {
        public BuildCommandArgs(string path, IEnumerable<string> targets, string buildType, bool configure,
                                bool noConfigure, IEnumerable<string> buildProperties, string output)
        {
            Path = path;
            Targets = targets;
            BuildType = buildType;
            Configure = configure;
            NoConfigure = noConfigure;
            BuildProperties = buildProperties;
            Output = output;
        }

        public string Path { get; }
        public IEnumerable<string> Targets { get; }
        public string BuildType { get; }
        public bool Configure { get; }
        public bool NoConfigure { get; }
        public IEnumerable<string> BuildProperties { get; }
        public string Output { get; }
    }
}
