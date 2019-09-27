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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using SharpCompress.Archives.Tar;
using SharpCompress.Compressors.Xz;

namespace PlcNext.Common.Tools.IO
{
    internal class SharpZipFileUnpackService : IFileUnpackService
    {
        private readonly IFileSystem fileSystem;
        private readonly StringBuilder archiveResultBuilder = new StringBuilder();
        private readonly IEnvironmentService environmentService;
        private readonly IProcessManager processManager;
        private readonly ILog log;

        public SharpZipFileUnpackService(IFileSystem fileSystem, IEnvironmentService environmentService, IProcessManager processManager, ILog log)
        {
            this.fileSystem = fileSystem;
            this.environmentService = environmentService;
            this.processManager = processManager;
            this.log = log;
        }

        public async Task Unpack(VirtualFile file, VirtualDirectory destination,
                                 IProgressNotifier parentProgress = null, ChangeObservable observable = null)
        {
            if (Path.GetExtension(file.Name)?.Equals(".zip", StringComparison.OrdinalIgnoreCase) == true)
            {
                await UnpackZipFile().ConfigureAwait(false);
            }
            else if (file.Name.EndsWith(".tar.xz", StringComparison.OrdinalIgnoreCase))
            {
                UnpackTarXzFile(file);
            }
            else if (Path.GetExtension(file.Name)?.Equals(".xz", StringComparison.OrdinalIgnoreCase) == true)
            {
                UnpackXzFile(file);
            }
            else if (Path.GetExtension(file.Name)?.Equals(".tar", StringComparison.OrdinalIgnoreCase)==true)
            {
                UnpackTarFile(file);
            }
            else if (Path.GetExtension(file.Name)?.Equals(".sh", StringComparison.OrdinalIgnoreCase) == true &&
                     environmentService.Platform != OSPlatform.Windows)
            {
                await UnpackSelfExtractingShellScript().ConfigureAwait(false);
            }
            else
            {
                throw new UnsupportedArchiveFormatException(file.FullName);
            }

            async Task UnpackSelfExtractingShellScript()
            {
                ValidateShellScript();
                StringBuilderUserInterface userInterface = new StringBuilderUserInterface(log, writeInformation: true, writeError: true);
                try
                {
                    using (parentProgress?.SpawnInfiniteProgress("Executing the shell script."))
                    using (IProcess process = processManager.StartProcess(file.FullName, $"-y -d \"{destination.FullName}\"", userInterface))
                    {
                        await process.WaitForExitAsync().ConfigureAwait(false);
                        if (process.ExitCode != 0)
                        {
                            throw new UnsupportedArchiveFormatException(file.FullName,
                                                                        new FormattableException(
                                                                            $"An error occured while executing the script.{Environment.NewLine}" +
                                                                            $"{userInterface.Error}"));
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is FormattableException)
                    {
                        throw;
                    }
                    throw new UnsupportedArchiveFormatException(file.FullName,
                                                                new FormattableException(
                                                                    $"An exception occured while executing the script.{Environment.NewLine}" +
                                                                    $"{e.Message}", e));
                }

                void ValidateShellScript()
                {
                    StringBuilderUserInterface validationUserInterface = new StringBuilderUserInterface(log, writeInformation: true, writeError: true);
                    try
                    {
                        using (IProcess process = processManager.StartProcess(file.FullName, "--help", validationUserInterface))
                        {
                            process.WaitForExit();
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        throw new UnsupportedArchiveFormatException(file.FullName,
                                                                    new FormattableException(
                                                                        $"An exception occured while inspecting the script.{Environment.NewLine}" +
                                                                        $"This excpetion can occur when the file is not marked as executable.{Environment.NewLine}" +
                                                                        $"{e.Message}", e));
                    }
                    catch (Exception e)
                    {
                        if (e is FormattableException)
                        {
                            throw;
                        }
                        throw new UnsupportedArchiveFormatException(file.FullName,
                                                                    new FormattableException(
                                                                        $"An exception occured while inspecting the script.{Environment.NewLine}" +
                                                                        $"{e.Message}", e));
                    }

                    if (!Regex.IsMatch(validationUserInterface.Information,
                                       $@"(?=.*(?:usage|Usage)).*{Regex.Escape(file.Name)}(?=.*-y)(?=.*-d)"))
                    {
                        throw new UnsupportedArchiveFormatException(file.FullName,
                            new FormattableException($"Did not find the expected usage information.{Environment.NewLine}" +
                                                     $"The expected information need to include the options '-y' and '-d'.{Environment.NewLine}" +
                                                     $"The following usage information was given:{Environment.NewLine}" +
                                                     $"{validationUserInterface.Information}"));
                    }
                }
            }

            async Task UnpackZipFile()
            {
                using (Stream fileStream = file.OpenRead())
                using (ZipFile zipFile = new ZipFile(fileStream))
                using (IProgressNotifier mainProgress = parentProgress?.Spawn(2, $"Extracting {file.FullName} to {destination.FullName}."))
                {
                    archiveResultBuilder.Clear();
                    using (mainProgress?.SpawnInfiniteProgress("Test archive integrity."))
                    {
                        ZipFile copy = zipFile;
                        await Task.Run(() =>
                        {
                            if (!copy.TestArchive(true, TestStrategy.FindAllErrors, ResultHandler))
                            {
                                throw new UnsupportedArchiveFormatException(
                                    file.FullName, new FormattableException(archiveResultBuilder.ToString()));
                            }
                        }).ConfigureAwait(false);
                    }

                    double increment = (double)Constants.ProgressMaxResolution / zipFile.Count + 1;
                    using (IProgressNotifier extractProgress = mainProgress?.Spawn(Constants.ProgressMaxResolution, "Extract files"))
                    {
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            extractProgress?.TickIncrement(increment);
                            if (!zipEntry.IsFile)
                            {
                                continue;           // Ignore directories
                            }

                            byte[] buffer = new byte[Constants.StreamCopyBufferSize];     // 4K is optimum

                            using (Stream zipStream = zipFile.GetInputStream(zipEntry))
                            {
                                string[] path = fileSystem.GetPath(zipEntry.Name);
                                VirtualDirectory fileDestination = destination.Directory(path.Take(path.Length - 1).ToArray());
                                VirtualFile entryFile = fileDestination.File(path.Last());

                                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                                // of the file, but does not waste memory.
                                using (Stream streamWriter = entryFile.OpenWrite())
                                {
                                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                                }

                            }
                        }
                    }
                }
            }

            VirtualFile UnpackXzFile(VirtualFile packedFile)
            {
                using (Stream fileStream = packedFile.OpenRead())
                using (XZStream xzStream = new XZStream(fileStream))
                using (parentProgress?.SpawnInfiniteProgress($"Extracting {packedFile.Name}..."))
                {
                    string[] path = fileSystem.GetPath(packedFile.FullName);
                    string relativeFilePath = path.Last().Substring(0, path.Last().LastIndexOf(".xz", StringComparison.OrdinalIgnoreCase));
                    if (destination.FileExists(relativeFilePath))
                    {
                        destination.File(relativeFilePath).Delete();
                    }
                    VirtualFile destinationFile = destination.File(relativeFilePath);
                    observable?.OnNext(new Change(() => destinationFile.Restore()));

                    byte[] buffer = new byte[Constants.StreamCopyBufferSize];     // 4K is optimum

                    using (Stream streamWriter = destinationFile.OpenWrite())
                    {
                        StreamUtils.Copy(xzStream, streamWriter, buffer);
                        return destinationFile;
                    }
                }
            }

            void UnpackTarFile(VirtualFile packedFile)
            {
                //sharpcompress
                using (Stream fileStream = packedFile.OpenRead())
                using (TarArchive tarArchive = TarArchive.Open(fileStream))
                {
                    double increment = (double)Constants.ProgressMaxResolution / tarArchive.Entries.Count;
                    using (IProgressNotifier extractProgress = parentProgress?.Spawn(Constants.ProgressMaxResolution, "Extracting .tar archive"))
                    {
                        foreach (TarArchiveEntry tarEntry in tarArchive.Entries)
                        {
                            extractProgress?.TickIncrement(increment);
                            if (tarEntry.IsDirectory)
                            {
                                continue;           // Ignore directories
                            }

                            byte[] buffer = new byte[Constants.StreamCopyBufferSize];     // 4K is optimum

                            using (Stream tarStream = tarEntry.OpenEntryStream())
                            {
                                string[] path = fileSystem.GetPath(tarEntry.Key);
                                VirtualDirectory fileDestination = destination.Directory(path.Take(path.Length - 1).ToArray());
                                if (fileDestination.FileExists(path.Last()))
                                {
                                    fileDestination.File(path.Last()).Delete();
                                }
                                VirtualFile entryFile = fileDestination.File(path.Last());
                                observable?.OnNext(new Change(() => entryFile.Restore()
                                //, $"Unpack {tarEntry.Key}."
                                ));

                                //Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                                // of the file, but does not waste memory.
                                using (Stream streamWriter = entryFile.OpenWrite())
                                {
                                    StreamUtils.Copy(tarStream, streamWriter, buffer);
                                }
                            }
                        }
                    }
                }
            }

            void UnpackTarXzFile(VirtualFile packedFile)
            {
                using (IProgressNotifier subProgress = parentProgress?.Spawn(2))
                {
                    parentProgress = subProgress;
                    VirtualFile tarFile = UnpackXzFile(packedFile);
                    UnpackTarFile(tarFile);
                    tarFile.Delete();
                }
            }
        }

        private void ResultHandler(TestStatus status, string message)
        {
            archiveResultBuilder.AppendLine(message);
        }
    }
}
