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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using NSubstitute;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedFileSystemAbstraction : IFileSystemAbstraction
    {
        public IFileSystem FileSystem { get; } = Substitute.For<IFileSystem>();

        private readonly IDictionary<VirtualDirectory, HashSet<VirtualEntry>> directoryContents = new Dictionary<VirtualDirectory, HashSet<VirtualEntry>>();
        private readonly IDictionary<VirtualFile, MemoryStream> fileContents = new Dictionary<VirtualFile, MemoryStream>();
        private readonly IDictionary<string, MemoryStream> deletedFileContents = new Dictionary<string, MemoryStream>();
        private readonly HashSet<VirtualFile> initialFiles = new HashSet<VirtualFile>();
        private readonly HashSet<VirtualFile> changedFiles = new HashSet<VirtualFile>();
        private readonly HashSet<string> throwOnAccessFiles = new HashSet<string>();
        private bool create;
        private Action<string> printMessage;

        public MockedFileSystemAbstraction()
        {
            VirtualDirectory current = CreateDirectory("Root", Path.Combine(Environment.CurrentDirectory, "Root"));

            FileSystem.CurrentDirectory.Returns(current);
            FileSystem.GetDirectory(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(callInfo =>
                                                                GetDirectory(callInfo.ArgAt<string>(0), callInfo.ArgAt<string>(1)));
            FileSystem.GetPath(null).ReturnsForAnyArgs((info) => info
                                                                .Arg<string>().Split(
                                                                     new[]
                                                                     {
                                                                         Path.DirectorySeparatorChar,
                                                                         Path.AltDirectorySeparatorChar
                                                                     },
                                                                     StringSplitOptions.RemoveEmptyEntries));

            FileSystem.IsRooted(Arg.Any<string>()).Returns(info => Path.IsPathRooted(info.Arg<string>()));

            FileSystem.DirectoryExists(Arg.Any<string>(), Arg.Any<string>()).Returns(callinfo => DirectoryExists(callinfo.ArgAt<string>(0), callinfo.ArgAt<string>(1)));

            FileSystem.FileExists(Arg.Any<string>(), Arg.Any<string>()).Returns(callinfo => FileExists(callinfo.ArgAt<string>(0), callinfo.ArgAt<string>(1)));

            FileSystem.GetFile(Arg.Any<string>(), Arg.Any<string>())
                      .Returns(callinfo =>
                      {
                          string path = Clean(callinfo.ArgAt<string>(0));
                          return GetDirectory(Path.GetDirectoryName(path), callinfo.ArgAt<string>(1))
                                     .File(Path.GetFileName(path));
                      });

            FileSystem.GetTemporaryDirectory().Returns(callinfo => GetTemporaryFolder());
        }

        public void RemoveApplicationFile(string fileName)
        {
            FileSystem.GetFile(Path.Combine(AssemblyDirectory, fileName)).Delete();
        }

        public string MoveApplicationFile(string fileName, string newLocation)
        {
            VirtualFile oldFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, fileName));
            VirtualFile newFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, newLocation));
            using (Stream source = oldFile.OpenRead())
            using (Stream destination = newFile.OpenWrite())
            {
                source.CopyTo(destination);
            }

            return newFile.FullName;
        }

        public Stream GetDeletedFileStream(string filePath)
        {
            deletedFileContents.ContainsKey(filePath).Should().BeTrue($"File {filePath} was expected as deleted file, but deleted files are:{Environment.NewLine}{string.Join(Environment.NewLine, deletedFileContents.Keys)}");
            MemoryStream memoryStream = new MemoryStream();
            deletedFileContents[filePath].Seek(0, SeekOrigin.Begin);
            deletedFileContents[filePath].CopyTo(memoryStream);
            memoryStream.SetLength(deletedFileContents[filePath].Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private VirtualDirectory CreateDirectory(string name, string fullName)
        {
            IDirectoryContentResolver resolver = Substitute.For<IDirectoryContentResolver>();
            VirtualDirectory directory = new VirtualDirectory(name, resolver);
            SetupDirectoryResolver(resolver, directory);
            directoryContents[directory] = new HashSet<VirtualEntry>();
            return directory;

            void SetupDirectoryResolver(IDirectoryContentResolver directoryContentResolver, VirtualDirectory currentDirectory)
            {
                directoryContentResolver.FullName.Returns(fullName);
                directoryContentResolver.Created.Returns(create);
                directoryContentResolver.GetContent().Returns((callinfo) => directoryContents[currentDirectory]);
                directoryContentResolver.Create<VirtualDirectory>(null).ReturnsForAnyArgs(info =>
                {
                    VirtualDirectory child = CreateDirectory(info.Arg<string>(), Path.Combine(fullName, info.Arg<string>()));
                    printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: Create {child} in {currentDirectory.FullName}");
                    directoryContents[currentDirectory].Add(child);
                    return child;
                });
                directoryContentResolver.Create<VirtualFile>(null)
                                        .ReturnsForAnyArgs(info =>
                                        {
                                            VirtualFile result = CreateFile(info.Arg<string>(), Path.Combine(fullName, info.Arg<string>()));
                                            printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: Create {result} in {currentDirectory.FullName}");
                                            directoryContents[currentDirectory].Add(result);
                                            return result;
                                        });
                directoryContentResolver.When(r => r.Delete()).Do((info) =>
                {
                    printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: Delete {currentDirectory}");
                    directoryContents.Remove(currentDirectory);
                });
                directoryContentResolver.CreatePath(null).ReturnsForAnyArgs(info => Path.Combine(info.Arg<string[]>()));
                directoryContentResolver.SplitPath(null).ReturnsForAnyArgs(info => info.Arg<string>().Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                                                                                                            StringSplitOptions.RemoveEmptyEntries));

                VirtualFile CreateFile(string fileName, string fileFullName)
                {
                    IFileContentResolver fileContentResolver = Substitute.For<IFileContentResolver>();
                    VirtualFile file = new VirtualFile(fileName, fileContentResolver);
                    fileContentResolver.FullName.Returns(fileFullName);
                    fileContentResolver.Created.Returns(create);
                    fileContentResolver.CheckAccess().Returns(true);
                    fileContents[file] = new MemoryStream();
                    fileContentResolver.GetContent()
                                       .ReturnsForAnyArgs(info => GetFileContent(file, info.ArgAt<bool>(0)));
                    fileContentResolver.When(r => r.Delete()).Do((info) =>
                    {
                        if (fileContents.ContainsKey(file))
                        {
                            deletedFileContents[file.FullName] = fileContents[file];
                        }
                        printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: Delete {file}");
                        fileContents.Remove(file);
                    });
                    fileContentResolver.When(r => r.UnDelete()).Do(info =>
                    {
                        fileContents[file] = new MemoryStream();
                        printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: Undelete {file}");
                    });
                    fileContentResolver.CreatePath(null).ReturnsForAnyArgs(info => Path.Combine(info.Arg<string[]>()));
                    fileContentResolver.SplitPath(null).ReturnsForAnyArgs(info => info.Arg<string>().Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                                                                                                                StringSplitOptions.RemoveEmptyEntries));
                    return file;
                }
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private Stream GetFileContent(VirtualFile file, bool write = false)
        {
            if (throwOnAccessFiles.Contains(file.FullName))
            {
                throw new FormattableException("You shall not pass.");
            }
            MemoryStream fileContent = fileContents[file];
            fileContent.Seek(0, SeekOrigin.Begin);
            ClosedStream closedStream = new ClosedStream();
            fileContent.CopyTo(closedStream);
            closedStream.SetLength(fileContent.Length);
            closedStream.Seek(0, SeekOrigin.Begin);
            closedStream.Closing += OnClosing;
            if (write)
            {
                changedFiles.Add(file);
            }
            return closedStream;

            void OnClosing(object sender, EventArgs args)
            {
                closedStream.Closing -= OnClosing;
                if (write)
                {
                    closedStream.Seek(0, SeekOrigin.Begin);
                    fileContent.Seek(0, SeekOrigin.Begin);
                    closedStream.CopyTo(fileContent);
                    fileContent.SetLength(closedStream.Length);
                }
            }
        }

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            this.printMessage = printMessage;
            
            exportProvider.AddInstance(FileSystem);
            
            SetupSdks();

            SetupKeyFiles();

            SetupLibraryBuilder();

            Load("Templates", FileSystem.GetDirectory(AssemblyDirectory), ".meta", "._acf");

            create = true;

            void SetupKeyFiles()
            {
                VirtualFile keyFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, "public_cli_repository_key.xml"));
                using (Stream fileStream = keyFile.OpenWrite())
                using (Stream resourcStream = Assembly.GetExecutingAssembly()
                                                      .GetManifestResourceStream("Test.PlcNext.Deployment.StandardPublicCliRepositoryKey.xml"))
                {
                    fileStream.SetLength(0);
                    resourcStream.CopyTo(fileStream);
                }
            }

            void SetupSdks()
            {
                string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                VirtualFile settingsFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, assemblyName, "settings.xml"));
                using (Stream fileStream = settingsFile.OpenWrite())
                using (Stream resourcStream = Assembly.GetExecutingAssembly()
                                                 .GetManifestResourceStream("Test.PlcNext.Deployment.StandardSettings.xml"))
                using (StreamReader reader = new StreamReader(resourcStream))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    fileStream.SetLength(0);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            line = line.Replace(@"C:/MySdk", "/usr/bin/MySdk");
                        }
                        writer.WriteLine(line);
                    }
                }

                VirtualDirectory sdkDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                                    ? FileSystem.GetDirectory(@"C:\MySdk")
                                                    : FileSystem.GetDirectory(@"/usr/bin/MySdk");
                VirtualFile propertiesFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, assemblyName, "sdk-properties.xml"));
                using (Stream fileStream = propertiesFile.OpenWrite())
                using (Stream resourcStream = Assembly.GetExecutingAssembly()
                                                      .GetManifestResourceStream("Test.PlcNext.Deployment.StandardSdkProperties.xml"))
                using (StreamReader reader = new StreamReader(resourcStream))
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    fileStream.SetLength(0);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            line = line.Replace(@"C:\MySdk", "/usr/bin/MySdk");
                        }
                        writer.WriteLine(line);
                    }
                }
            }

            void SetupLibraryBuilder()
            {
                //HACK - implicit creates file
                FileSystem.GetFile(Path.Combine(AssemblyDirectory, "bin", "LibraryBuilder.Core"));
                VirtualFile fileNamesFile = FileSystem.GetFile(Path.Combine(AssemblyDirectory, "file-names.xml"));
                using (Stream fileStream = fileNamesFile.OpenWrite())
                using (Stream resourcStream = Assembly.GetExecutingAssembly()
                                                      .GetManifestResourceStream("Test.PlcNext.Deployment.StandardFileNames.xml"))
                {
                    resourcStream.CopyTo(fileStream);
                }
            }
        }

        private readonly Dictionary<string, VirtualDirectory> roots = new Dictionary<string, VirtualDirectory>();

        private string Clean(string path)
        {
            path = path ?? string.Empty;
            return path.Trim('"')
                       .Replace('\\', Path.DirectorySeparatorChar)
                       .Replace('/', Path.DirectorySeparatorChar);
        }

        public VirtualDirectory GetDirectory(string path, string basePath)
        {
            path = Clean(path);
            basePath = Clean(basePath);

            if (!string.IsNullOrEmpty(basePath) && !Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(Path.Combine(basePath, path));
            }

            if (string.IsNullOrEmpty(path))
            {
                return CurrentDirectory;
            }

            if (path.StartsWith(CurrentDirectory.FullName))
            {
                if (path.Length <= CurrentDirectory.FullName.Length + 1)
                {
                    return CurrentDirectory;
                }

                path = path.Substring(CurrentDirectory.FullName.Length + 1);
            }

            VirtualDirectory rootDirectory = CurrentDirectory;
            if (Path.IsPathRooted(path))
            {
                string root = Path.GetPathRoot(path);
                if (roots.ContainsKey(root))
                {
                    rootDirectory = roots[root];
                }
                else
                {
                    rootDirectory = CreateDirectory(root, root);
                    roots.Add(root, rootDirectory);
                }

                path = Path.GetRelativePath(root, path);
            }
            else
            {
                VirtualDirectory foundRoot = roots.FirstOrDefault(r => path.StartsWith(r.Key)).Value;
                if (foundRoot != null)
                {
                    rootDirectory = foundRoot;
                    path = Path.GetRelativePath(rootDirectory.FullName, path);
                }
            }
            string[] parts = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                                 .SkipWhile(p => p == ".")
                                 .ToArray();
            return parts.Take(parts.Length).Aggregate(rootDirectory, (current, part) => current.Directory(part));
        }

        public Stream Open(string path)
        {
            (VirtualDirectory directory, string fileName) = FindDirectory(path);

            if (directory == null || string.IsNullOrEmpty(fileName) || !directory.FileExists(fileName))
            {
                printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: File {path} not found. [{directory}] Available structure{Environment.NewLine}{PrintStructure()}");
                return null;
            }
            return GetFileContent(directory.File(fileName));
        }

        private string PrintStructure()
        {
            StringBuilder builder = new StringBuilder();
            string tab = "\t";
            builder.AppendLine($"{CurrentDirectory}");
            Print(CurrentDirectory, builder, tab);
            return builder.ToString();
        }

        private void Print(VirtualDirectory currentDirectory, StringBuilder builder, string tab)
        {
            foreach (VirtualEntry entry in currentDirectory.Entries())
            {
                builder.AppendLine($"{tab}{entry}");
                if (entry is VirtualDirectory directory)
                {
                    Print(directory, builder, tab + '\t');
                }
            }
        }

        private (VirtualDirectory directory, string fileName) FindDirectory(string path, string basePath = "")
        {
            if (path.StartsWith(CurrentDirectory.FullName))
            {
                if (path.Length <= CurrentDirectory.FullName.Length + 1)
                {
                    return (CurrentDirectory, string.Empty);
                }

                path = path.Substring(CurrentDirectory.FullName.Length + 1);
            }

            VirtualDirectory rootDirectory = GetDirectory(basePath, "");
            if (Path.IsPathRooted(path))
            {
                string root = Path.GetPathRoot(path);
                rootDirectory = roots.ContainsKey(root) ? roots[root] : null;
                path = Path.GetRelativePath(root, path);
            }
            else
            {
                VirtualDirectory foundRoot = roots.FirstOrDefault(r => path.StartsWith(r.Key)).Value;
                if (foundRoot != null)
                {
                    rootDirectory = foundRoot;
                    path = Path.GetRelativePath(rootDirectory.FullName, path);
                }
            }

            if (rootDirectory == null)
            {
                return (null, string.Empty);
            }
            string[] parts = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Any() && parts[0] == ".")
            {
                parts = parts.Skip(1).ToArray();
            }
            VirtualDirectory directory = parts.Take(parts.Length - 1)
                                              .Aggregate(rootDirectory,
                                                         (current, part) =>
                                                             current != null
                                                                 ? (VirtualDirectory) directoryContents[current]
                                                                    .FirstOrDefault(e => e.Name == part)
                                                                 : null);
            string fileName = parts.LastOrDefault();
            return (directory, fileName);
        }

        public VirtualDirectory CurrentDirectory => FileSystem.CurrentDirectory;

        public bool FileExists(string path, string basePath = "")
        {
            path = path.CleanPath();
            (VirtualDirectory directory, string fileName) = FindDirectory(path, basePath);
            return !string.IsNullOrEmpty(fileName) && directory?.FileExists(fileName) == true;
        }

        public bool FindFile(ref string path)
        {
            (VirtualDirectory directory, string fileName) = FindDirectory(path);
            VirtualFile foundFile = !string.IsNullOrEmpty(fileName)
                                        ? directory?.Files(fileName, true).FirstOrDefault()
                                        : null;
            if (foundFile != null)
            {
                path = foundFile.FullName;
                return true;
            }
            return false;
        }

        private readonly HashSet<VirtualDirectory> temporaryDirectories = new HashSet<VirtualDirectory>();

        public VirtualDirectory GetTemporaryFolder(bool createNew = true)
        {
            VirtualDirectory directory;
            if (createNew)
            {
                string name = Guid.NewGuid().ToByteString();
                directory = CreateDirectory(name, name);
                roots.Add(name, directory);
                temporaryDirectories.Add(directory);
            }
            else
            {
                directory = temporaryDirectories.Single();
            }

            return directory;
        }

        public bool DirectoryExists(string path, string basePath)
        {
            (VirtualDirectory directory, string directoryName) = FindDirectory(path, basePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                return directory != null;
            }
            return directory?.DirectoryExists(directoryName) == true;
        }

        public void Load(string workspace)
        {
            Load(workspace, CurrentDirectory);
        }

        private void Load(string workspace, VirtualDirectory destination, params string[] unsplittables)
        {
            create = false;
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (string resource in assembly.GetManifestResourceNames()
                                                .Where(s => s.StartsWith($"{assembly.GetName().Name}.Deployment.{workspace}.",
                                                                         StringComparison.OrdinalIgnoreCase)))
            {
                VirtualDirectory current = destination;

                //string[] path = resource.Substring($"{assembly.GetName().Name}.Deployment.{workspace}.".Length).Split(new[] {'.'});
                string subResourceId = resource.Substring($"{assembly.GetName().Name}.Deployment.{workspace}.".Length);
                subResourceId = Escape(subResourceId);
                string[] path = Regex.Split(subResourceId, @"\.(?!_)");
                current = current.Directory(workspace);
                for (int i = 0; i < path.Length - 2; i++)
                {
                    current = current.Directory(Unescape(path[i]));
                }

                using (Stream fileStream = current.File($"{Unescape(path[path.Length - 2])}.{Unescape(path[path.Length - 1])}").OpenWrite())
                {
                    using (Stream resourceStream = assembly.GetManifestResourceStream(resource))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }

            changedFiles.Clear();
            initialFiles.Clear();
            foreach (VirtualFile file in fileContents.Keys)
            {
                initialFiles.Add(file);
            }

            create = true;

            string Unescape(string part)
            {
                for (int j = 0; j < unsplittables.Length; j++)
                {
                    part = part.Replace($"unsplittables{j}", unsplittables[j]);
                }
                part = part.Replace("._", ".");
                part = part.Replace("_c_", ",");

                return part;
            }

            string Escape(string subResourceId)
            {
                foreach (string unsplittable in unsplittables.Where(subResourceId.Contains))
                {
                    int index = Array.IndexOf(unsplittables, unsplittable);
                    subResourceId = subResourceId.Replace(unsplittable, $"unsplittables{index}");
                }

                return subResourceId;
            }
        }

        public void Load(Stream content, string fileName)
        {
            create = false;
            using (Stream fileStream = FileSystem.GetFile(fileName).OpenWrite())
            {
                content.CopyTo(fileStream);
            }

            create = true;
        }

        public void LoadInto(string workspace, string destination)
        {
            VirtualDirectory destinationDirectory = FileSystem.GetDirectory(destination, CurrentDirectory.FullName);
            Load(workspace, destinationDirectory);
        }

        public IEnumerable<string> CreatedFiles => fileContents.Keys.Except(initialFiles)
                                                               .Where(f => f.FullName.StartsWith(CurrentDirectory.FullName))
                                                               .Select(f => f.Name);
        public IEnumerable<string> ChangedFiles => changedFiles.Intersect(initialFiles).Select(f => f.Name);
        public IEnumerable<string> DeletedFiles => initialFiles.Except(fileContents.Keys).Select(f => f.Name);
        public void ThrowOnAccess(string path)
        {
            //if virtualfiles are stored in throwonaccessfiles instead of strings, the files will be created here
            //-> problems in transaction feature
            throwOnAccessFiles.Add(Path.Combine(CurrentDirectory.FullName, path));
        }

        public void SetCurrentDirectory(string path)
        {
            string[] parts = path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                                        StringSplitOptions.RemoveEmptyEntries);
            VirtualDirectory currentDirectory = parts.Aggregate(CurrentDirectory,
                                                                (current, part) => current.Directory(part));
            FileSystem.CurrentDirectory.Returns(currentDirectory);
        }

        public void RemoveSdk()
        {
            FileSystem.GetDirectory(@"C:\MySdk").Delete();
        }

        private sealed class ClosedStream : MemoryStream
        {
            public event EventHandler<EventArgs> Closing; 
            public override void Close()
            {
                OnClosing();
                base.Close();
            }

            private void OnClosing()
            {
                Closing?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            foreach (MemoryStream stream in fileContents.Values)
            {
                stream.Dispose();
            }
            foreach (MemoryStream stream in deletedFileContents.Values)
            {
                stream.Dispose();
            }
        }
    }
}
