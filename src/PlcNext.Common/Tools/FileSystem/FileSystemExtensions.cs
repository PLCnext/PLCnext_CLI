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
using System.Text;
using PlcNext.Common.Tools.IO;

namespace PlcNext.Common.Tools.FileSystem
{
    public static class FileSystemExtensions
    {
        public static string GetRelativeOrAbsolutePath(this VirtualFile file, VirtualDirectory directory)
        {
            string result = file.GetRelativePath(directory);
            if (result.StartsWith(".."))
            {
                return file.FullName;
            }

            return result;
        }

        public static Exception Format(this Exception exception)
        {
            switch (exception)
            {
                case IOException ioException:
                    return new FormattableIoException(ioException);
                case UnauthorizedAccessException unauthorizedAccessException:
                    return new FormattableIoException(unauthorizedAccessException);
                default:
                    return exception;
            }
        }
        
        public static Stream OpenComparingWriteStream(this VirtualFile file)
        {
            return new ComparingStream(file);
        }

        public static string CleanPath(this string path)
        {
            path = path ?? string.Empty;
            return path.Trim('"').Trim()
                       .Replace('\\', Path.DirectorySeparatorChar)
                       .Replace('/', Path.DirectorySeparatorChar);
        }

        private class ComparingStream : Stream
        {
            private readonly VirtualFile file;
            private readonly Stream internalStream;

            public ComparingStream(VirtualFile file)
            {
                this.file = file;
                internalStream = RecyclableMemoryStreamManager.Instance.GetStream();
            }

            public override void Flush()
            {
                internalStream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return internalStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return internalStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                internalStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                internalStream.Write(buffer, offset, count);
            }

            public override bool CanRead => internalStream.CanRead;

            public override bool CanSeek => internalStream.CanSeek;

            public override bool CanWrite => internalStream.CanWrite;

            public override long Length => internalStream.Length;

            public override long Position
            {
                get => internalStream.Position;
                set => internalStream.Position = value;
            }

            private void CompareAndCopy()
            {
                internalStream.Seek(0, SeekOrigin.Begin);
                bool copy = false;
                using (Stream fileStream = file.OpenRead())
                {
                    if (internalStream.Length != fileStream.Length)
                    {
                        copy = true;
                    }
                    else
                    {
                        byte[] fileBuffer = new byte[Constants.StreamCopyBufferSize];
                        byte[] internalBuffer = new byte[Constants.StreamCopyBufferSize];
                        while (fileStream.Read(fileBuffer, 0, fileBuffer.Length) > 0)
                        {
                            internalStream.Read(internalBuffer, 0, internalBuffer.Length);
                            if (!fileBuffer.SequenceEqual(internalBuffer))
                            {
                                copy = true;
                                break;
                            }
                        }
                    }
                }

                if (copy)
                {
                    internalStream.Seek(0, SeekOrigin.Begin);
                    using (Stream fileStream = file.OpenWrite())
                    {
                        fileStream.SetLength(internalStream.Length);
                        internalStream.CopyTo(fileStream);
                    }
                }
            }

            private bool disposed;

            protected override void Dispose(bool disposing)
            {
                if (disposing && ! disposed)
                {
                    CompareAndCopy();
                    internalStream.Dispose();
                    disposed = true;
                }
                base.Dispose(disposing);
            }
        }
    }
}
