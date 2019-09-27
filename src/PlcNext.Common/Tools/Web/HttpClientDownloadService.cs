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
using System.Net.Http;
using System.Threading.Tasks;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Web
{
    internal class HttpClientDownloadService : IDownloadService
    {
        private readonly ISettingsProvider settingsProvider;

        public HttpClientDownloadService(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public Task Download(Uri url, Stream writeStream, IProgressNotifier parentProgress = null)
        {
            return Download(url, writeStream,
                            string.IsNullOrEmpty(settingsProvider.Settings.HttpProxy)
                                ? null
                                : new WebProxy(new Uri(settingsProvider.Settings.HttpProxy)),
                            parentProgress);
        }

        public async Task Download(Uri url, Stream writeStream, IWebProxy webProxy, IProgressNotifier parentProgress = null)
        {
            using (HttpClientHandler clientHandler = new HttpClientHandler())
            using (IProgressNotifier progress = parentProgress?.Spawn(1.0, $"Download {url}"))
            {
                if (webProxy != null)
                {
                    clientHandler.Proxy = webProxy;
                }

                try
                {
                    using (HttpClient client = new HttpClient(clientHandler))
                    {
                        await client.DownloadAsync(url, writeStream, progress).ConfigureAwait(false);
                    }
                }
                catch (HttpRequestException e)
                {
                    throw new DownloadFileNotReachableException(url, e);
                }
            }
        }
    }
}
