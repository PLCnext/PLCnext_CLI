#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.Web
{
    internal static class HttpClientExtensions
    {
        //from https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient
        //progress should have max of 1.0 as in 100%
        public static async Task DownloadAsync(this HttpClient client, Uri requestUri, Stream destination, IProgressNotifier progress = null)
        {
            // Get the http headers first to examine the content length
            using (HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                long? contentLength = response.Content.Headers.ContentLength;

                using (Stream download = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {

                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination).ConfigureAwait(false);
                        return;
                    }
                    
                    // Use extension method to report progress while downloading
                    await download.CopyToAsync(destination, Report).ConfigureAwait(false);

                    // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                    void Report(long totalBytesCompleted) => progress.Tick((double)totalBytesCompleted / contentLength.Value);
                }
            }
        }
    }
}
