#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.UI
{
    public interface IProgressNotifier : IDisposable
    {
        void TickIncrement(double addedProgress = 1.0, string message = "");

        void Tick(double totalProgress, string message = "");

        IProgressNotifier Spawn(double maxTicks, string startMessage = "");

        IDisposable SpawnInfiniteProgress(string startMessage);
    }
}
