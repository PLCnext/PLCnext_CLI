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
    /// <summary>
    /// Interface for a page
    /// </summary>
    public interface IPage : IDisposable
    {
        /// <summary>
        /// Gets the size of a memory block.
        /// </summary>
        long Capacity { get; }

        /// <summary>
        /// Resizes the page to the new capacity.
        /// </summary>
        /// <param name="newCapacity">A capacity of a new <see cref="IPage">. </see>/> </param>
        /// <returns>The <see cref="IPage"/>.</returns>
        IPage Resize(long newCapacity);

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position
        /// within this stream by the number of bytes written. </summary>
        /// <param name="offset"> A byte offset relative to the origin parameter.</param>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="bufferOffset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <returns>The current position.</returns>
        int CopyTo(long offset, byte[] buffer, int bufferOffset, int count);

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position
        /// within the stream by the number of bytes read. </summary>
        /// <param name="offset">A byte offset relative to the origin parameter</param>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="bufferOffset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream</param>
        void CopyFrom(long offset, byte[] buffer, int bufferOffset, int count);

        /// <summary>
        /// Clears all buffers for this stream and causes
        /// any buffered data to be written to the underlying device.
        /// </summary>
        void Flush();
    }
}
