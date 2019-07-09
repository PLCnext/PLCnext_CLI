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
    internal class CommunicationSettings
    {
        public CommunicationSettings(bool heartbeatEnabled)
        {
            HeartbeatEnabled = heartbeatEnabled;
        }

        public bool HeartbeatEnabled { get; }
    }
}
