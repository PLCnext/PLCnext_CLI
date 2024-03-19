#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Commands
{
    public class GetProjectInformationCommandArgs : CommandArgs
    {

        public GetProjectInformationCommandArgs(string path, IEnumerable<string> sources, bool noIncludePathDetection, string buildType)
        {
            Path = path;
            Sources = sources;
            NoIncludePathDetection = noIncludePathDetection;
            BuildType = buildType;
        }

        public string Path { get; }
        public IEnumerable<string> Sources { get; }
        public bool NoIncludePathDetection { get; }
        public string BuildType { get; }
    }
}
