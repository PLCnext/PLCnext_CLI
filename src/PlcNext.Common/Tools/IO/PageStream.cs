#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;

namespace PlcNext.Common.Tools.IO
{
    internal class PageStream : Stream
    {
#pragma warning disable CA2213 // Disposable fields should be disposed
        private IPage page;
#pragma warning restore CA2213 // Disposable fields should be disposed
        private long length;
        private long position;

        public PageStream(IPage page, long length)
        {
            this.page = page ?? throw new ArgumentNullException(nameof(page));
            this.length = length;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => length;

        public override long Position
        {
            get => position;

            set => position = value;
        }

        public override void Flush()
        {
            Position = 0;
            page.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            long readCount = Math.Min(length - position, count);

            int result = page.CopyTo(position, buffer, offset, (int)readCount);
            position += result;
            return result;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            long newLength = Math.Max(position, 0) + count;
            if (newLength > Length - 1)
            {
                SetLength(newLength);
            }

            page.CopyFrom(position, buffer, offset, count);
            position += count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.Current:
                    position = position + offset;
                    break;
                case SeekOrigin.End:
                    position = Math.Max(length - 1 - offset, 0);
                    break;
            }

            return position;
        }

        public override void SetLength(long value)
        {
            IPage newPage = page.Resize(value);
            length = value;
            position = Math.Min(length - 1, Math.Max(position, 0));
            page = newPage;
        }
    }
}
