#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.IO
{
    internal class ConcurrentPage : IPage
    {
        private readonly IPage page;
        private readonly object lockToken;

        public ConcurrentPage(IPage page, object lockToken)
        {
            this.page = page ?? throw new ArgumentNullException(nameof(page));
            this.lockToken = lockToken ?? throw new ArgumentNullException(nameof(lockToken));
        }

        public long Capacity
        {
            get
            {
                lock (lockToken)
                {
                    return page.Capacity;
                }
            }
        }

        public int CopyTo(long offset, byte[] buffer, int bufferOffset, int count)
        {
            lock (lockToken)
            {
                return page.CopyTo(offset, buffer, bufferOffset, count);
            }
        }

        public void CopyFrom(long offset, byte[] buffer, int bufferOffset, int count)
        {
            lock (lockToken)
            {
                page.CopyFrom(offset, buffer, bufferOffset, count);
            }
        }

        public IPage Resize(long newCapactity)
        {
            lock (lockToken)
            {
                IPage newPage = page.Resize(newCapactity);
                if (newPage == null)
                {
                    return null;
                }
                else
                {
                    return new ConcurrentPage(newPage, lockToken);
                }
            }
        }

        public void Flush()
        {
            lock (lockToken)
            {
                page.Flush();
            }
        }

        public void Dispose()
        {
            lock (lockToken)
            {
                page.Dispose();
            }
        }
    }
}
