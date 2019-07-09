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
    internal class ConcurrentPageBuffer : IPageBuffer
    {
        private readonly IPageBuffer pageBuffer;
        private readonly object lockToken;

        public ConcurrentPageBuffer(IPageBuffer pageBuffer, object lockToken)
        {
            this.pageBuffer = pageBuffer ?? throw new ArgumentNullException(nameof(pageBuffer));
            this.lockToken = lockToken ?? throw new ArgumentNullException(nameof(lockToken));
        }

        public IPage Allocate(long capacity)
        {
            lock (lockToken)
            {
                IPage page = pageBuffer.Allocate(capacity);

                if (page == null)
                {
                    return null;
                }
                else
                {
                    return new ConcurrentPage(page, lockToken);
                }
            }
        }

        public IPage Create(long capacity)
        {
            lock (lockToken)
            {
                IPage page = pageBuffer.Allocate(capacity);

                if (page == null)
                {
                    return null;
                }
                else
                {
                    return new ConcurrentPage(page, lockToken);
                }
            }
        }

        public void Clear()
        {
            lock (lockToken)
            {
                pageBuffer.Clear();
            }
        }

        public void Dispose()
        {
            lock (lockToken)
            {
                pageBuffer.Dispose();
            }
        }
    }
}
