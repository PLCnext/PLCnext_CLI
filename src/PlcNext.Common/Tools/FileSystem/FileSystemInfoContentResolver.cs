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
using System.Text;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.FileSystem
{
    internal abstract class FileSystemInfoContentResolver : IEntryContentResolver
    {
        private readonly FileSystemInfo entryInfo;
        protected readonly ILog Log;
        public bool Created { get; }

        protected FileSystemInfoContentResolver(FileSystemInfo entryInfo, bool created, ILog log)
        {
            this.entryInfo = entryInfo;
            this.Log = log;
            Created = created;
        }

        public string FullName => entryInfo.FullName;

        public virtual void Delete()
        {
            try
            {
                if (entryInfo.Exists)
                {
                    entryInfo.Delete();
                }
            }
            catch (Exception e)
            {
                throw e.Format();
            }
        }

        public abstract void UnDelete();
    }
}
