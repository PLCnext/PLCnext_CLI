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

        public GetProjectInformationCommandArgs(string path, IEnumerable<string> sources)
        {
            Path = path;
            Sources = sources;
        }

        public string Path { get; }
        public IEnumerable<string> Sources { get; }
    }
}
