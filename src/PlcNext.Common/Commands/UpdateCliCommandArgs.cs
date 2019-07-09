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
    public class UpdateCliCommandArgs : CommandArgs
    {
        public UpdateCliCommandArgs(string version, string proxy, string file)
        {
            Version = version;
            Proxy = proxy;
            File = file;
        }

        public string Version { get; }
        public string Proxy { get; }
        public string File { get; }
    }
}
