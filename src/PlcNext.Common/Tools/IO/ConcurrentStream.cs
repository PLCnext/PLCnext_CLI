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
    /// <summary>
    /// A decorator to make a stream thread safe.
    /// </summary>
    public class ConcurrentStream : Stream
    {
        private readonly Stream innerStream;
        private readonly object lockToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner stream.</param>
        /// <param name="lockToken">The underlying lock token.</param>
        public ConcurrentStream(Stream innerStream, object lockToken)
        {
            this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            this.lockToken = lockToken ?? throw new ArgumentNullException(nameof(lockToken));
        }

        /// <inheritdoc/>
        public override bool CanRead
        {
            get
            {
                lock (lockToken)
                {
                    return innerStream.CanRead;
                }
            }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get
            {
                lock (lockToken)
                {
                    return innerStream.CanSeek;
                }
            }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get
            {
                lock (lockToken)
                {
                    return innerStream.CanSeek;
                }
            }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                lock (lockToken)
                {
                    return innerStream.Length;
                }
            }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                lock (lockToken)
                {
                    return innerStream.Position;
                }
            }

            set
            {
                lock (lockToken)
                {
                    innerStream.Position = value;
                }
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (lockToken)
            {
                return innerStream.Seek(offset, origin);
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            lock (lockToken)
            {
                return innerStream.Read(buffer, offset, count);
            }
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            lock (lockToken)
            {
                innerStream.SetLength(value);
            }
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            lock (lockToken)
            {
                innerStream.Write(buffer, offset, count);
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            lock (lockToken)
            {
                innerStream.Flush();
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                innerStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
