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
    /// Interface defines a page buffer.
    /// </summary>
    public interface IPageBuffer : IDisposable
    {
        /// <summary>
        /// Allocate a memory segment of this buffer and return <see cref="IPage"/>
        /// of speicifed size.
        /// </summary>
        /// <param name="capacity">A size of new allocated <see cref="IPage"/>.</param>
        /// <returns>The <see cref="IPage"/>.</returns>
        IPage Allocate(long capacity);

        /// <summary>
        /// Creates a new instance of <see cref="IPage"/> with define capacity.
        /// </summary>
        /// <param name="capacity">A size of new allocated <see cref="IPage"/>.</param>
        /// <returns>The <see cref="IPage"/>.</returns>
        IPage Create(long capacity);

        /// <summary>
        /// Clear the buffer from allocated memory blocks.
        /// </summary>
        void Clear();
    }
}
