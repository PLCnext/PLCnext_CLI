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
    public class GetProgramsCommandArgs : CommandArgs
    {
        public GetProgramsCommandArgs(string path, string component, IEnumerable<string> sources)
        {
            Path = path;
            Component = component;
            Sources = sources;
        }

        public string Path { get; }

        public string Component { get; }
        public IEnumerable<string> Sources { get; }
    }
}
