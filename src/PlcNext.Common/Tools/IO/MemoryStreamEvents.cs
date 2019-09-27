#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.Tracing;

namespace PlcNext.Common.Tools.IO
{
    /// <summary>
    /// 
    /// </summary>
    [EventSource(Name = "Microsoft-IO-RecyclableMemoryStream", Guid = "{B80CD4E4-890E-468D-9CBA-90EB7C82DFC7}")]
    public sealed class MemoryStreamEvents : EventSource
    {
        /// <summary>
        /// 
        /// </summary>
        public new static readonly MemoryStreamEvents Write = new MemoryStreamEvents();

        /// <summary>
        /// 
        /// </summary>
        public enum MemoryStreamBufferType
        {
            /// <summary>
            /// 
            /// </summary>
            Small,
            /// <summary>
            /// 
            /// </summary>
            Large
        }


        /// <summary>
        /// 
        /// </summary>
        public enum MemoryStreamDiscardReason
        {
            /// <summary>
            /// 
            /// </summary>
            TooLarge,
            /// <summary>
            /// 
            /// </summary>
            EnoughFree
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="requestedSize"></param>
        [Event(1, Level = EventLevel.Verbose)]
        public void MemoryStreamCreated(Guid id, string tag, int requestedSize)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(1, id, tag ?? String.Empty, requestedSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        [Event(2, Level = EventLevel.Verbose)]
        public void MemoryStreamDisposed(Guid id, string tag)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(2, id, tag ?? String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="allocationStack"></param>
        /// <param name="disposeStack1"></param>
        /// <param name="disposeStack2"></param>
        [Event(3, Level = EventLevel.Critical)]
        public void MemoryStreamDoubleDispose(Guid id, string tag, string allocationStack, string disposeStack1,
                                              string disposeStack2)
        {
            if (IsEnabled())
            {
                WriteEvent(3, id, tag ?? String.Empty, allocationStack ?? String.Empty,
                           disposeStack1 ?? String.Empty, disposeStack2 ?? String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="allocationStack"></param>
        [Event(4, Level = EventLevel.Error)]
        public void MemoryStreamFinalized(Guid id, string tag, string allocationStack)
        {
            if (IsEnabled())
            {
                WriteEvent(4, id, tag ?? String.Empty, allocationStack ?? String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="stack"></param>
        /// <param name="size"></param>
        [Event(5, Level = EventLevel.Verbose)]
        public void MemoryStreamToArray(Guid id, string tag, string stack, int size)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(5, id, tag ?? String.Empty, stack ?? String.Empty, size);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockSize"></param>
        /// <param name="largeBufferMultiple"></param>
        /// <param name="maximumBufferSize"></param>
        [Event(6, Level = EventLevel.Informational)]
        public void MemoryStreamManagerInitialized(int blockSize, int largeBufferMultiple, int maximumBufferSize)
        {
            if (IsEnabled())
            {
                WriteEvent(6, blockSize, largeBufferMultiple, maximumBufferSize);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallPoolInUseBytes"></param>
        [Event(7, Level = EventLevel.Verbose)]
        public void MemoryStreamNewBlockCreated(long smallPoolInUseBytes)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(7, smallPoolInUseBytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiredSize"></param>
        /// <param name="largePoolInUseBytes"></param>
        [Event(8, Level = EventLevel.Verbose)]
        public void MemoryStreamNewLargeBufferCreated(int requiredSize, long largePoolInUseBytes)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(8, requiredSize, largePoolInUseBytes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requiredSize"></param>
        /// <param name="tag"></param>
        /// <param name="allocationStack"></param>
        [Event(9, Level = EventLevel.Verbose)]
        public void MemoryStreamNonPooledLargeBufferCreated(int requiredSize, string tag, string allocationStack)
        {
            if (IsEnabled(EventLevel.Verbose, EventKeywords.None))
            {
                WriteEvent(9, requiredSize, tag ?? String.Empty, allocationStack ?? String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferType"></param>
        /// <param name="tag"></param>
        /// <param name="reason"></param>
        [Event(10, Level = EventLevel.Warning)]
        public void MemoryStreamDiscardBuffer(MemoryStreamBufferType bufferType, string tag,
                                              MemoryStreamDiscardReason reason)
        {
            if (IsEnabled())
            {
                WriteEvent(10, bufferType, tag ?? String.Empty, reason);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedCapacity"></param>
        /// <param name="maxCapacity"></param>
        /// <param name="tag"></param>
        /// <param name="allocationStack"></param>
        [Event(11, Level = EventLevel.Error)]
        public void MemoryStreamOverCapacity(long requestedCapacity, long maxCapacity, string tag,
                                             string allocationStack)
        {
            if (IsEnabled())
            {
                WriteEvent(11, requestedCapacity, maxCapacity, tag ?? String.Empty, allocationStack ?? String.Empty);
            }
        }
    }
}

// This is here for .NET frameworks which don't support EventSource. We basically shim bare functionality used above to  
#if FAKEEVENTSOURCE
namespace System.Diagnostics.Tracing
{
    public enum EventLevel
    {
        LogAlways = 0,
        Critical,
        Error,
        Warning,
        Informational,
        Verbose,
    }

    public enum EventKeywords : long
    {
        None = 0x0,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EventSourceAttribute : Attribute
    {
        public string Name { get; set; }
        public string Guid { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EventAttribute : Attribute
    {
        public EventAttribute(int id) { }

        public EventLevel Level { get; set; }
    }

    public class EventSource
    {
        public void WriteEvent(params object[] unused)
        {
            return;
        }

        public bool IsEnabled()
        {
            return false;
        }

        public bool IsEnabled(EventLevel level, EventKeywords keywords)
        {
            return false;
        }
    }
}
#endif
