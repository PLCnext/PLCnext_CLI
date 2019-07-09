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
    public class GetAppComponentsCommandArgs : CommandArgs
    {
        public GetAppComponentsCommandArgs(string path, IEnumerable<string> sources)
        {
            Path = path;
            Sources = sources;
        }

        public string Path { get; }
        public IEnumerable<string> Sources { get; }
    }
}
