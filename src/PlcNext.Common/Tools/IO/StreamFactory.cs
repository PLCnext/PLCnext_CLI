#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;

namespace PlcNext.Common.Tools.IO
{
    /// <summary>
    /// Defines a abstract class for a Stream Factory.
    /// </summary>
    public abstract class StreamFactory
    {
        /// <summary>
        /// Creates a Stream of specified length.
        /// </summary>
        /// <param name="length">Defines a length of a Stream</param>
        /// <returns>A <see cref="Stream"/></returns>
        public abstract Stream Create(long length);
    }
}
