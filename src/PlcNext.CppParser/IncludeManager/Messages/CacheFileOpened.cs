#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.IO;
using Agents.Net;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    public class CacheFileOpened : Message
    {
        public CacheFileOpened(Message predecessorMessage, Stream cacheFileStream): base(predecessorMessage)
        {
            CacheFileStream = cacheFileStream;
        }

        public CacheFileOpened(IEnumerable<Message> predecessorMessages, Stream cacheFileStream): base(predecessorMessages)
        {
            CacheFileStream = cacheFileStream;
        }
        
        public Stream CacheFileStream { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CacheFileStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
