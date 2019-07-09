#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Web
{
    /// <summary>
    /// Checks availability of download sites and downloads files for web adresses.
    /// </summary>
    internal interface IDownloadService
    {
        Task Download(Uri url, Stream writeStream, IProgressNotifier parentProgress = null);

        Task Download(Uri url, Stream writeStream, IWebProxy webProxy, IProgressNotifier parentProgress = null);
    }
}
