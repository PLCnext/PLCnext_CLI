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
using System.Threading;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools.FileSystem
{
    internal class FileInfoContentResolver : FileSystemInfoContentResolver, IFileContentResolver
    {
        private readonly FileInfo fileInfo;
        private MemoryStream recyclableMemoryStream;

        public FileInfoContentResolver(FileInfo fileInfo, bool created, ILog log) : base(fileInfo, created, log)
        {
            this.fileInfo = fileInfo;
        }

        public override void Delete()
        {
            EnsureMemento();
            base.Delete();
        }

        public Stream GetContent(bool write = false, bool retry = false)
        {
            EnsureMemento();
            try
            {
                if (fileInfo.Directory?.Exists == false)
                {
                    fileInfo.Directory.Create();
                }
            }
            catch (Exception e)
            {
                throw e.Format();
            }

            Exception exception = null;
            for (int i = 0; i < (retry? Constants.MaxIORetries : 1); i++)
            {
                try
                {
                    return fileInfo.Open(FileMode.OpenOrCreate, write ? FileAccess.Write : FileAccess.Read);
                }
                catch (Exception e)
                {
                    if (e is IOException || e is UnauthorizedAccessException)
                    {
                        exception = e;
                        Thread.Sleep(200);
                        continue;
                    }
                    throw;
                }
            }
            throw (exception ?? new IOException()).Format();
        }

        private void EnsureMemento()
        {
            if (!Created && fileInfo.Exists && recyclableMemoryStream == null)
            {
                try
                {
                    recyclableMemoryStream = RecyclableMemoryStreamManager.Instance.GetStream();
                    using (Stream stream = fileInfo.OpenRead())
                    {
                        stream.CopyTo(recyclableMemoryStream);
                    }
                }
                catch (Exception e)
                {
                    recyclableMemoryStream?.Dispose();
                    recyclableMemoryStream = null;
                    Log.LogError($"Exception while reading file {this.fileInfo.FullName}. " +
                                 $"File cannot be restored on error.{Environment.NewLine}{e}");
                }
            }
        }

        public void Touch()
        {
            try
            {
                fileInfo.LastWriteTime = DateTime.Now;
            }
            catch (Exception e)
            {
                throw e.Format();
            }
        }

        public DateTime LastWriteTime => fileInfo.LastWriteTime;
        public bool CheckAccess()
        {
            fileInfo.Refresh();
            if (fileInfo.Exists)
            {
                //Check if file can be opened
                FileStream readStream = null;
                FileStream writeStream = null;

                try
                {
                    readStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                    readStream.Close();
                    writeStream = fileInfo.Open(FileMode.Open, FileAccess.Write, FileShare.None);
                }
                catch (IOException)
                {
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                finally
                {
                    readStream?.Close();
                    writeStream?.Close();
                }
                return true;
            }
            else
            {
                //Check if random file can be created
                string file = Path.Combine(fileInfo.DirectoryName??string.Empty, $"{Guid.NewGuid()}.{Guid.NewGuid()}");
                bool canCreate;
                try
                {
                    using (File.Create(file)) { }
                    File.Delete(file);
                    canCreate = true;
                }
                catch
                {
                    canCreate = false;
                }

                return canCreate;
            }
        }

        public override void UnDelete()
        {
            if (recyclableMemoryStream != null)
            {
                using (Stream stream = fileInfo.Create()) 
                {
                    recyclableMemoryStream.WriteTo(stream);
                }
                recyclableMemoryStream.Close();
                recyclableMemoryStream = null;
            }
        }
    }
}
