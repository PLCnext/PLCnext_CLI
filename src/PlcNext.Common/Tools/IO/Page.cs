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
    internal class Page : IPage
    {
        private Block block;
        private BuddyAllocator allocator;

        public Page(BuddyAllocator allocator, Block block)
        {
            this.block = block ?? throw new ArgumentNullException(nameof(block));
            this.allocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
        }

        public long Capacity => block.Capacity;

        public void CopyFrom(long pageOffset, byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            allocator.Stream.Seek(block.Offset + pageOffset, SeekOrigin.Begin);
            allocator.Stream.Write(buffer, offset, count);
        }

        public int CopyTo(long pageOffset, byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            allocator.Stream.Seek(block.Offset + pageOffset, SeekOrigin.Begin);

            return allocator.Stream.Read(buffer, offset, count);
        }

        public IPage Resize(long newCapacity)
        {
            if (newCapacity <= Capacity)
            {
                return this;
            }

            Page newPage = allocator.AllocateCore(newCapacity);
            allocator.BlockCopy(block.Offset, newPage.block.Offset, Math.Min(Capacity, newCapacity));
            DisposeCore();
            return newPage;
        }

        public void Dispose()
        {
            if (block != null)
            {
                DisposeCore();
            }
        }

        public void Flush()
        {
            allocator.FlushStream();
        }

        private void DisposeCore()
        {
            block.Release();
            block = null;
        }
    }
}
