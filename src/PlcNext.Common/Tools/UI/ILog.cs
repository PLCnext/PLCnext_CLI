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

namespace PlcNext.Common.Tools.UI
{
    public interface ILog
    {
        void LogVerbose(string message);
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}
