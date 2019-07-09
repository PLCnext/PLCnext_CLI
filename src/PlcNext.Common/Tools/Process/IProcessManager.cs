#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Process
{
    /// <summary>
    /// Starts and manages external processes
    /// </summary>
    internal interface IProcessManager : IProcessInformationService
    {
        IProcess StartProcess(string fileName, string arguments,
                              IUserInterface userInterface,
                              string workingDirectory = null, bool showOutput = true, bool showError = true,
                              bool killOnDispose = true);
    }
}
