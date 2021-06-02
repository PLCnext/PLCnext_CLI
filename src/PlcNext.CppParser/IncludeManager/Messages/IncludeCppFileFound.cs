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
    //This is the decorator for parsing new files to add to the cache
    public class IncludeCppFileFound : MessageDecorator
    {
        private IncludeCppFileFound(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static IncludeCppFileFound Decorate(CppFileFound decoratedMessage,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new IncludeCppFileFound(decoratedMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
