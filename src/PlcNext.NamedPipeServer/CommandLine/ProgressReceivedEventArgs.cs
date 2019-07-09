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
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal class ProgressReceivedEventArgs : EventArgs
    {
        public ProgressReceivedEventArgs(CommandProgress progress)
        {
            Progress = progress;
        }

        public CommandProgress Progress { get; }
    }
}
