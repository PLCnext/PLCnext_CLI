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
    public interface IProgressVisualizer
    {
        IProgressNotifier Spawn(double maxTicks, string startMessage, string completedMessage);

        IDisposable SpawnInfiniteProgress(string startMessage);
    }
}
