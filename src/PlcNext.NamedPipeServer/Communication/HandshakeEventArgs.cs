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

namespace PlcNext.NamedPipeServer.Communication
{
    internal class HandshakeEventArgs : EventArgs
    {
        public HandshakeEventArgs(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}
