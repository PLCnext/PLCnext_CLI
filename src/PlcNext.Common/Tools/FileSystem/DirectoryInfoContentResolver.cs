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
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.FileSystem
{
    internal class DirectoryInfoContentResolver : FileSystemInfoContentResolver, IDirectoryContentResolver
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly StringComparison pathEquality;

        public DirectoryInfoContentResolver(DirectoryInfo directoryInfo, bool created, ILog log,
                                            StringComparison pathEquality) : base(directoryInfo, created, log)
        {
            this.directoryInfo = directoryInfo;
            this.pathEquality = pathEquality;
        }

        public IEnumerable<VirtualEntry> GetContent()
        {
            try
            {
                return directoryInfo.GetFileSystemInfos()
                                    .Select(CreateEntry);
            }
            catch (Exception e)
            {
                throw e.Format();
            }

            VirtualEntry CreateEntry(FileSystemInfo info)
            {
                if (info is DirectoryInfo directory)
                {
                    return new VirtualDirectory(directory.Name, new DirectoryInfoContentResolver(directory, false, Log, pathEquality), pathEquality);
                }

                return new VirtualFile(info.Name, new FileInfoContentResolver((FileInfo) info, false, Log));
            }
        }

        public T Create<T>(string name) where T : VirtualEntry
        {
            VirtualEntry entry;
            try
            {
                if (typeof(T) == typeof(VirtualDirectory))
                {
                    entry = new VirtualDirectory(name, new DirectoryInfoContentResolver(directoryInfo.CreateSubdirectory(name), true, Log, pathEquality), pathEquality);
                }
                else if(typeof(T) == typeof(VirtualFile))
                {
                    FileInfo info = new FileInfo(Path.Combine(directoryInfo.FullName, name));
                    info.Create().Close();
                    entry = new VirtualFile(name, new FileInfoContentResolver(info, true, Log));
                }
                else
                {
                    throw new ArgumentException($@"Type {typeof(T)} is not allowed", "T");
                }
            }
            catch (Exception e)
            {
                throw e.Format();
            }

            return (T) entry;
        }

        public override void UnDelete()
        {
            directoryInfo.Create();
        }
    }
}
