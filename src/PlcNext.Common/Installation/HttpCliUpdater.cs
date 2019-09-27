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
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.Security;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Tools.Web;

namespace PlcNext.Common.Installation
{
    internal class HttpCliUpdater : ICliUpdater
    {
        private readonly ISettingsProvider settingsProvider;
        private readonly IDownloadService downloadService;
        private readonly IFileSystem fileSystem;
        private readonly ISecurityValidator securityValidator;
        private readonly IFileUnpackService fileUnpackService;
        private readonly IUserInterface userInterface;
        private readonly IEnvironmentService environmentService;
        private readonly IBinariesLocator binariesLocator;
        private readonly IProcessManager processManager;
        private readonly IEnumerable<IInstallationStep> installationSteps;

        private Repository repository;

        public HttpCliUpdater(ISettingsProvider settingsProvider, IDownloadService downloadService,
                              IFileSystem fileSystem, ISecurityValidator securityValidator,
                              IFileUnpackService fileUnpackService, IUserInterface userInterface,
                              IEnvironmentService environmentService,
                              IProcessManager processManager, 
                              IBinariesLocator binariesLocator, IEnumerable<IInstallationStep> installationSteps)
        {
            this.settingsProvider = settingsProvider;
            this.downloadService = downloadService;
            this.fileSystem = fileSystem;
            this.securityValidator = securityValidator;
            this.fileUnpackService = fileUnpackService;
            this.userInterface = userInterface;
            this.environmentService = environmentService;
            this.processManager = processManager;
            this.binariesLocator = binariesLocator;
            this.installationSteps = installationSteps;
        }

        private async Task LoadRepository(string proxy)
        {
            Uri baseUri = InvalidUriFormatException.TryCreateUri(settingsProvider.Settings.CliRepositoryRoot);
            Uri repositoryFile = InvalidUriFormatException.TryCreateUri(settingsProvider.Settings.CliRepositoryFileName, baseUri);
            Uri signatureFile = InvalidUriFormatException.TryCreateUri(settingsProvider.Settings.CliRepositorySignatureFileName, baseUri);
            string publicKeyFile = Path.Combine(environmentService.AssemblyDirectory, Constants.PublicKeyFileName);
            if (!fileSystem.FileExists(publicKeyFile))
            {
                throw new PublicKeyFileNotFoundException();
            }

            using (MemoryStream repositoryStream = RecyclableMemoryStreamManager.Instance.GetStream())
            using (MemoryStream signatureStream = RecyclableMemoryStreamManager.Instance.GetStream())
            using (Stream publicKeyFileStream = fileSystem.GetFile(publicKeyFile).OpenRead())
            {
                await Download(repositoryFile, repositoryStream, proxy).ConfigureAwait(false);
                repositoryStream.Seek(0, SeekOrigin.Begin);

                await Download(signatureFile, signatureStream, proxy).ConfigureAwait(false);
                signatureStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    securityValidator.ValidateSignature(repositoryStream, signatureStream, publicKeyFileStream);
                }
                catch (SignatureValidationException e)
                {
                    e.ValidationFileName = repositoryFile.AbsolutePath;
                    throw;
                }

                repositoryStream.Seek(0, SeekOrigin.Begin);
                XmlSerializer serializer = new XmlSerializer(typeof(Repository));
                using (XmlReader reader = XmlReader.Create(repositoryStream))
                {
                    repository = (Repository) serializer.Deserialize(reader);
                }
            }
        }

        private async Task Download(Uri repositoryFile, Stream repositoryStream, string proxy, IProgressNotifier parentProgress = null)
        {
            if (!string.IsNullOrEmpty(proxy))
            {
                await downloadService.Download(repositoryFile, repositoryStream, new WebProxy(new Uri(proxy)), parentProgress).ConfigureAwait(false);
            }
            else
            {
                await downloadService.Download(repositoryFile, repositoryStream, parentProgress).ConfigureAwait(false);
            }
        }

        public async Task<bool> IsCurrentVersion(string version, string proxy = "")
        {
            await EnsureRepository(proxy).ConfigureAwait(false);
            string actualVersion = await CheckVersion(version, proxy).ConfigureAwait(false);
            bool onlyUpdate = actualVersion != version;
            CliVersionDefinition versionDefinition = repository.Version.FirstOrDefault(v => v.GetInformalVersion() == actualVersion);
            if (versionDefinition != null)
            {
                Version formalVersion = Version.Parse(versionDefinition.version);

                return onlyUpdate
                           ? formalVersion <= environmentService.AssemblyVersion
                           : formalVersion == environmentService.AssemblyVersion;
            }

            return false;
        }

        private async Task<string> CheckVersion(string version, string proxy)
        {
            if (string.IsNullOrEmpty(version))
            {
                await EnsureRepository(proxy).ConfigureAwait(false);
                version = repository.Version.OrderByDescending(v => Version.Parse(v.version)).FirstOrDefault()
                                   ?.GetInformalVersion() ?? string.Empty;
            }

            return version;
        }

        public async Task<VirtualFile> DownloadVersion(string version, VirtualDirectory downloadDirectory,
                                                       IProgressNotifier parentProgress = null,
                                                       string proxy = "")
        {
            await EnsureRepository(proxy).ConfigureAwait(false);
            string actualVersion = await CheckVersion(version, proxy).ConfigureAwait(false);

            CliVersionDefinition versionDefinition = repository.Version.FirstOrDefault(v => v.GetInformalVersion() == actualVersion);
            if (versionDefinition == null)
            {
                throw new UnkownVersionException(actualVersion);
            }

            FileDefinition downloadFile = GetCorrectVersion();
            if (downloadFile == null)
            {
                throw new UnsupportedSystemException(environmentService.PlatformName, environmentService.Architecture);
            }

            Uri downloadUri = InvalidUriFormatException.TryCreateUri(downloadFile.relPath,
                                                                     InvalidUriFormatException.TryCreateUri(settingsProvider.Settings.CliRepositoryRoot));
            VirtualFile result = downloadDirectory.File(downloadFile.name);
            using (Stream fileStream = result.OpenWrite())
            {
                await Download(downloadUri, fileStream, proxy, parentProgress).ConfigureAwait(false);
            }

            using (Stream fileStream = result.OpenRead())
            {
                try
                {
                    securityValidator.ValidateHash(fileStream, downloadFile.Fingerprint.hash, downloadFile.Fingerprint.algorithm);
                }
                catch (HashValidationException e)
                {
                    e.ValidationFileName = downloadUri.AbsolutePath;
                    throw;
                }
            }

            return result;

            FileDefinition GetCorrectVersion()
            {
                FileDefinition[] correctArchitecture = versionDefinition.File
                                                                        .Where(f => f.Architecture.ToString().Equals(environmentService.Architecture, 
                                                                                                                     StringComparison.OrdinalIgnoreCase))
                                                                        .ToArray();
                return correctArchitecture.FirstOrDefault(f => f.OS.ToString().Equals(environmentService.PlatformName,
                                                              StringComparison.OrdinalIgnoreCase)) ??
                       correctArchitecture.FirstOrDefault(f => f.OS == OSDefinition.unbound);
            }
        }

        public async Task InstallVersion(VirtualFile setup, VirtualDirectory tempDirectory, IProgressNotifier parentProgress = null)
        {
            VirtualDirectory destination = tempDirectory.Directory(Guid.NewGuid().ToByteString());

            using (IProgressNotifier progressNotifier = parentProgress?.Spawn(installationSteps.Count()+1,"Installing version"))
            {
                await fileUnpackService.Unpack(setup, destination, progressNotifier).ConfigureAwait(false);

                foreach (IInstallationStep installationStep in installationSteps)
                {
                    await installationStep.Install(destination, progressNotifier).ConfigureAwait(false);
                }
            
                await ReplaceApplication().ConfigureAwait(false);
            }

            async Task ReplaceApplication()
            {
                userInterface.WriteInformation("Replace current version with new version");

                if (environmentService.Platform != OSPlatform.Windows)
                {
                    await destination.CopyToAsync(fileSystem.GetDirectory(environmentService.AssemblyDirectory)).ConfigureAwait(false);
                    tempDirectory.Delete();
                }
                else
                {
                    VirtualFile batchFile = tempDirectory.File("copy_version.bat");
                    Assembly currentAssembly = Assembly.GetAssembly(GetType());
                    using (Stream batchResourceStream = currentAssembly.GetManifestResourceStream("PlcNext.Common.Installation.copy_version.bat"))
                    using (Stream batchFileStream = batchFile.OpenWrite())
                    {
                        batchResourceStream?.CopyTo(batchFileStream);
                    }

                    processManager.StartProcess(batchFile.FullName,
                                                $"\"{destination.FullName}\" \"{environmentService.AssemblyDirectory}\" " +
                                                $"\"{tempDirectory.FullName}\" \"{binariesLocator.GetExecutable("application").Name}\"", userInterface, null, showOutput: false, showError: false, killOnDispose: false);
                }
            }
        }

        public async Task<IEnumerable<Version>> GetAvailableVersions(string proxy = "")
        {
            await EnsureRepository(proxy).ConfigureAwait(false);
            return repository.Version.Select(v => Version.Parse(v.version));
        }

        private async Task EnsureRepository(string proxy)
        {
            if (repository == null)
            {
                await LoadRepository(proxy).ConfigureAwait(false);
            }
        }
    }
}
