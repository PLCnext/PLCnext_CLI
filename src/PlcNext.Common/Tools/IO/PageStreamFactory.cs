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
    /// The page stream factory.
    /// </summary>
    public class PageStreamFactory : StreamFactory, IDisposable
    {
        private readonly IPageBuffer pageBuffer;
        private readonly object lockToken = new object();
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageStreamFactory"/> class.
        /// It requires passing a <see cref="PageBuffer"/> for generation a Page with define length.
        /// </summary>
        /// <param name="pageBuffer">A <see cref="pageBuffer"/> </param>
        private PageStreamFactory(IPageBuffer pageBuffer)
        {
            if (pageBuffer == null)
                throw new ArgumentNullException(nameof(pageBuffer));

            this.pageBuffer = new ConcurrentPageBuffer(pageBuffer, lockToken);
        }

        public static PageStreamFactory CreateDefault(int gbCapacity = 1)
        {
            const int kb = 1024;
            const int mb = kb * 1024;
            const long gb = mb * 1024;
            PageBufferOptions pageBufferOptions =
                new PageBufferOptions(gbCapacity*gb, kb)
                {
                    BufferType = PageBufferType.File,
                    EnableDoubleBuffer = false
                };
            PageBuffer pageBuffer = new PageBuffer(pageBufferOptions);
            PageStreamFactory streamFactory = new PageStreamFactory(pageBuffer);
            return streamFactory;
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public override Stream Create(long length)
        {
            IPage page = pageBuffer.Create(length);

            if (page == null)
            {
                throw new ArgumentException(FormattableString.Invariant($"Cannot create a page of length {length}"), nameof(length));
            }

            return new ConcurrentStream(new PageStream(page, length), lockToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose allocated resources.
        /// </summary>
        /// <param name="disposing">If <c>true</c> resources are deposed, <c>false</c> otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!isDisposed)
                {
                    pageBuffer.Dispose();
                    isDisposed = true;
                }
            }
        }
    }
}