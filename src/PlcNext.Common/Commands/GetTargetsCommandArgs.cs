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
    public class GetTargetsCommandArgs : CommandArgs
    {
        public GetTargetsCommandArgs(string path, bool all, bool shortVersion)
        {
            Path = path;
            All = all;
            Short = shortVersion;
        }

        public string Path { get; }
        public bool All { get; }
        public bool Short { get; }
    }
}
