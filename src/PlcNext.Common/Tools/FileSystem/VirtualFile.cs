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
    }
}
