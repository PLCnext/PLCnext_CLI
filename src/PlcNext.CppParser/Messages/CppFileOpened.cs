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

namespace PlcNext.CppParser.Messages
{
    public class CppFileOpened : MessageDecorator
    {
        private CppFileOpened(Message decoratedMessage, Stream fileStream, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            FileStream = fileStream;
        }

        public static CppFileOpened Decorate(CppFileFound decoratedMessage, Stream fileStream,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new CppFileOpened(decoratedMessage, fileStream, additionalPredecessors);
        }
        
        public Stream FileStream { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FileStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
