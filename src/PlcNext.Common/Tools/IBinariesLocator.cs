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
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Tools
{
    public interface IBinariesLocator
    {
        VirtualFile GetExecutable(string executableKey, string baseDirectory = "");

        string GetExecutableCommand(string executableKey, string baseDirectory = "");
    }
}
