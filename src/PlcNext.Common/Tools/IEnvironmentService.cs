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
using System.Runtime.InteropServices;
using System.Text;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Tools
{
    public interface IEnvironmentService
    {
        Version AssemblyVersion { get; }
        string Architecture { get; }
        string PlatformName { get; }
        OSPlatform Platform { get; }
        string AssemblyDirectory { get; }
        IEnvironmentPathNames PathNames { get; }
    }
}
