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
    /// Defines a memory allocation strategy based on Buddy memory allocation algorithm used in Linux OS.
    /// It performs halving memory into partitions (buddy blocks) to try to satisfy a memory request as suitably as possible.
    /// </summary>
    public class BuddyAllocator
    {
        private readonly long minimumCapacity;
        private Stream stream;
        private Block root;
        private byte[] blockCopyBuffer = new byte[4096];

        /// <summary>
        /// Initializes a new instance of the <see cref="BuddyAllocator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="minimumCapacity">The minimum capacity</param>
        public BuddyAllocator(Stream stream, long capacity, long minimumCapacity)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.minimumCapacity = minimumCapacity;
            root = new Block { Capacity = capacity };
            this.stream.SetLength(capacity);
        }

        internal Stream Stream => stream;

        private bool IsDisposed => stream == null;

        /// <summary>
        /// Allocates memory block and return an <see cref="IPage"/> of define size.
        /// </summary>
        /// <param name="capacity">Size of an <see cref="IPage"/>.</param>
        /// <returns>An <see cref="IPage"> of define size.</see>/></returns>
        public IPage Allocate(long capacity)
        {
            EnsureNotDisposed();

            return AllocateCore(capacity);
        }

        /// <summary>
        /// Clears the blocks of memory.
        /// </summary>
        public void Clear()
        {
            EnsureNotDisposed();

            root.IsAllocated = false;
            root.Lower = null;
            root.Higher = null;
        }

        /// <summary>
        /// Clears all buffers for this stream and causes
        /// any buffered data to be written to the underlying device.
        /// </summary>
        public void FlushStream()
        {
            stream.Flush();
        }

        /// <summary>
        /// Closes the stream and dispose resources.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }
        }

        internal Page AllocateCore(long capacity)
        {
            EnsureNotDisposed();

            if (capacity == 0 && minimumCapacity == 0)
                return null;

            Block block = root.Allocate(Math.Max(capacity, minimumCapacity));

            if (block != null)
                return new Page(this, block);

            return null;
        }

        internal void BlockCopy(long sourcePosition, long destinationPosition, long count)
        {
            int remaining = (int)count;
            long offset = 0;

            while (remaining > 0)
            {
                stream.Seek(sourcePosition + offset, SeekOrigin.Begin);
                int read = stream.Read(blockCopyBuffer, 0, Math.Min(remaining, blockCopyBuffer.Length));
                stream.Seek(destinationPosition + offset, SeekOrigin.Begin);
                stream.Write(blockCopyBuffer, 0, read);
                remaining -= read;
                offset += read;
            }
        }

        private void EnsureNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(PageBuffer));
            }
        }
    }
}
