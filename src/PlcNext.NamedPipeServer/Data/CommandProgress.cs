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

namespace PlcNext.NamedPipeServer.Data
{
    internal class CommandProgress
    {
        public CommandProgress(Command command, string message, Guid progressId, double normalizedProgress)
        {
            Command = command;
            Message = message;
            ProgressId = progressId;
            NormalizedProgress = normalizedProgress;
        }

        public Command Command { get; }
        public string Message { get; }
        public Guid ProgressId { get; }
        public double NormalizedProgress { get; }
    }
}
