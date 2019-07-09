#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Tools.IO
{
    /// <summary>
    /// Options for the <see cref="PageBuffer"/>
    /// </summary>
    public class PageBufferOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageBufferOptions"/> class.
        /// </summary>
        /// <param name="capacity"><see cref="Capacity"/>.</param>
        /// <param name="minimalCapacity"><see cref="MinimalCapacity"/>.</param>
        public PageBufferOptions(long capacity, long minimalCapacity)
        {
            Capacity = capacity;
            MinimalCapacity = minimalCapacity;
            BufferType = PageBufferType.File;
            EnableDoubleBuffer = true;
        }

        /// <summary>
        /// Gets or sets the overall capacity of the page buffer.
        /// </summary>
        public long Capacity { get; set; }

        /// <summary>
        /// Gets or sets the minimal capacity of a page.
        /// </summary>
        public long MinimalCapacity { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BufferType"/>.
        /// </summary>
        public PageBufferType BufferType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable file double buffering.
        /// </summary>
        public bool EnableDoubleBuffer { get; set; }
    }
}
