#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools.UI;

namespace PlcNext.NamedPipeServer.CommandLine
{
    public interface ICommandLineBuilder
    {
        IDisposableCommandLineParser BuildCommandLineInstance(IUserInterface userInterface,
                                                              IProgressVisualizer progressVisualizer,
                                                              ICommandResultVisualizer commandResultVisualizer,
                                                              CancellationToken cancellationToken);
    }

    public interface IDisposableCommandLineParser : ICommandLineParser, IDisposable
    {

    }
}
