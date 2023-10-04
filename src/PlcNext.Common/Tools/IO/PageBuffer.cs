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
    /// <inheritdoc />
    public class PageBuffer : IPageBuffer
    {
        private BuddyAllocator buddyAllocactor;
        private Stream stream;
        private bool isDisposed = false;
        private readonly string tempFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBuffer"/> class.
        /// </summary>
        /// <param name="capacity">Size of the reserverd memory block</param>
        /// <param name="minimumCapacity">Minimum size of the memory block</param>
        public PageBuffer(long capacity, long minimumCapacity = 0)
            : this(new PageBufferOptions(capacity, minimumCapacity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBuffer"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public PageBuffer(PageBufferOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.BufferType == PageBufferType.File)
            {
                tempFile = System.IO.Path.GetTempFileName();
                stream = new FileStream(
                     System.IO.Path.GetTempFileName(),
                     FileMode.Create,
                     FileAccess.ReadWrite,
                     FileShare.None,
                     4096,
                     FileOptions.DeleteOnClose);

                if (options.EnableDoubleBuffer)
                {
                    stream = new BufferedStream(stream);
                }
            }
            else
            {
                tempFile = string.Empty;
                stream = new MemoryStream();
                stream.SetLength(options.Capacity);
            }

            buddyAllocactor = new BuddyAllocator(stream, options.Capacity, options.MinimalCapacity);
        }

        /// <inheritdoc />
        public IPage Create(long capacity)
        {
            return Allocate(capacity);
        }

        /// <inheritdoc />
        public IPage Allocate(long capacity)
        {
            return buddyAllocactor.Allocate(capacity);
        }

        /// <inheritdoc />
        public void Clear()
        {
            buddyAllocactor.Clear();
        }

        /// <summary>
        /// Dispose allocated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose allocated resources.
        /// </summary>
        /// <param name="disposing">If <c>true</c> the resources are disposed, <c>false</c> otherwise.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!isDisposed)
                {
                    stream.Dispose();
                    buddyAllocactor.Dispose();
                    if (!string.IsNullOrEmpty(tempFile) && File.Exists(tempFile))
                    {
                        try
                        {
                            File.Delete(tempFile);
                        }
                        catch (Exception)
                        {
                            //do nothing in dispose method
                        }
                    }
                    isDisposed = true;
                }
            }
        }
    }
}
