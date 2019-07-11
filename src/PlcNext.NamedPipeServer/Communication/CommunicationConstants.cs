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
    public static class CommunicationConstants
    {
        public static readonly Version[] SupportedProtocolVersions = {new Version(1, 0),};
        public const int ThreadJoinTimeout = 200;
        public const int HeartbeatInterval = 80;
        public const int ProgressResolution = 1000;
        public const int KillCancelWaitTimeout = 800;
        public const string MessageSenderContainerKey = "MessageSenderContainer";
    }
}
