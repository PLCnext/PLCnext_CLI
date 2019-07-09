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
using System.Threading.Tasks;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Installation
{
    internal interface ICliUpdater
    {
        Task<bool> IsCurrentVersion(string version, string proxy = "");

        Task<VirtualFile> DownloadVersion(string version, VirtualDirectory downloadDirectory,
                                          IProgressNotifier parentProgress = null, string proxy = "");

        Task InstallVersion(VirtualFile setup, VirtualDirectory toDeleteDirectory, IProgressNotifier parentProgress = null);
        
        Task<IEnumerable<Version>> GetAvailableVersions(string proxy = "");
    }
}
