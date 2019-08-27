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

namespace PlcNext.Common.Tools.FileSystem
{
    public class VirtualFile : VirtualEntry
    {
        private readonly IFileContentResolver contentResolver;

        public Stream OpenWrite(bool retry = false)
        {
            return contentResolver.GetContent(true, retry);
        }

        public Stream OpenRead(bool retry = false)
        {
            return contentResolver.GetContent(retry: retry);
        }

        public DateTime LastWriteTime => contentResolver.LastWriteTime;

        public VirtualFile(string name, IFileContentResolver contentResolver) : base(name, contentResolver)
        {
            this.contentResolver = contentResolver;
        }

        public void Touch()
        {
            contentResolver.Touch();
        }

        public bool CheckAccess()
        {
            return contentResolver.CheckAccess();
        }

        public string GetRelativePath(VirtualDirectory directory)
        {
            List<VirtualDirectory> reverseDirectoryPath = new List<VirtualDirectory>();
            VirtualDirectory current = directory;
            while (current != null)
            {
                reverseDirectoryPath.Add(current);
                current = current.Parent;
            }

            List<string> reverseFilePath = new List<string>(new[] {Name});
            current = Parent;
            while (current != null)
            {
                if (reverseDirectoryPath.Contains(current))
                {
                    foreach (VirtualDirectory _ in reverseDirectoryPath.TakeWhile(d => d != current))
                    {
                        reverseFilePath.Add("..");
                    }

                    reverseFilePath.Reverse();
                    return contentResolver.CreatePath(reverseFilePath.ToArray());
                }
                reverseFilePath.Add(current.Name);
                current = current.Parent;
            }

            return FullName;
        }
    }
}
