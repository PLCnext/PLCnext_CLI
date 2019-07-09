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
    public class InstallSdkCommandArgs : CommandArgs
    {
        public InstallSdkCommandArgs(string sdk, string destination, bool force)
        {
            Sdk = sdk;
            Destination = destination;
            Force = force;
        }

        public string Sdk { get; set; }

        public string Destination { get; set; }

        public bool Force { get; set; }
    }
}
