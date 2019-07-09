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
    public class SetTargetsCommandArgs : CommandArgs
    {

        public SetTargetsCommandArgs(string name, string version, bool add, bool remove, string path)
        {
            Name = name;
            Version = version;
            Add = add;
            Remove = remove;
            Path = path;
        }

        public string Name { get; }
        public string Version { get; }
        public bool Add { get; }
        public bool Remove { get; }
        public string Path { get; }

    }
}
