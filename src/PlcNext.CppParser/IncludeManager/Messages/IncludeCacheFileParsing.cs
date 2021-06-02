#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    //This is the decorator for parsing existing cache files when looking for a specific type
    public class IncludeCacheFileParsing : MessageDecorator
    {
        private IncludeCacheFileParsing(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static IncludeCacheFileParsing Decorate(CppFileFound decoratedMessage,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new IncludeCacheFileParsing(decoratedMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
