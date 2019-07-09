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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Web;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedDownloadServiceAbstraction : IDownloadServiceAbstraction
    {
        private readonly IDownloadService downloadService = Substitute.For<IDownloadService>();
        private readonly Dictionary<Uri, Stream> availableDownloads = new Dictionary<Uri, Stream>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MockedDownloadServiceAbstraction" /> class.
        /// </summary>
        public MockedDownloadServiceAbstraction()
        {
            downloadService.Download(null, null, null)
                           .ReturnsForAnyArgs(callinfo => ProcessDownload(callinfo.Arg<Uri>(), callinfo.Arg<Stream>()));

            downloadService.Download(null, null, null, null)
                           .ThrowsForAnyArgs(callinfo => new DownloadFileNotReachableException(callinfo.Arg<Uri>()));

            LoadCliRepository();

            void LoadCliRepository()
            {
                string basePath = "http://localhost/cli/";
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (string name in assembly.GetManifestResourceNames())
                {
                    if (!name.StartsWith("Test.PlcNext.Deployment.CliRepository."))
                    {
                        continue;
                    }

                    using (Stream mainfestStream = assembly.GetManifestResourceStream(name))
                    {
                        MemoryStream memoryStream = RecyclableMemoryStreamManager.Instance.GetStream();
                        mainfestStream.CopyTo(memoryStream);
                        availableDownloads.Add(BuildUrl(basePath, name.Replace("Test.PlcNext.Deployment.CliRepository.", "")),
                                               memoryStream);
                    }
                }
            }

            Uri BuildUrl(string basePath, string relativePath)
            {
                relativePath = relativePath.Substring(0, relativePath.LastIndexOf('.')).Replace('.', '/') +
                               relativePath.Substring(relativePath.LastIndexOf('.'));
                relativePath = relativePath.Replace("_point", ".").Replace("_remove", "");
                return new Uri(new Uri(basePath), new Uri(relativePath, UriKind.Relative));
            }
        }
        
        private async Task ProcessDownload(Uri path, Stream stream)
        {
            if (availableDownloads.ContainsKey(path))
            {
                stream.SetLength(0);
                Stream content = availableDownloads[path];
                content.Seek(0, SeekOrigin.Begin);
                await content.CopyToAsync(stream);
            }
            else
            {
                throw new DownloadFileNotReachableException(path);
            }
        }

        public void Initialize(InstancesRegistrationSource exportProvider)
        {
            exportProvider.AddInstance(downloadService);
        }

        public void RemoveFromServer(string path)
        {
            availableDownloads.Remove(new Uri(new Uri("http://localhost/"), new Uri(path, UriKind.Relative)));
        }

        public void ChangeFileContent(string path, Stream resourceStream)
        {
            Stream actualStream = availableDownloads[new Uri(new Uri("http://localhost/"), new Uri(path, UriKind.Relative))];
            actualStream.SetLength(0);
            resourceStream.CopyTo(actualStream);
        }

        public void Dispose()
        {
            foreach (Stream stream in availableDownloads.Values)
            {
                stream.Dispose();
            }
        }
    }
}
