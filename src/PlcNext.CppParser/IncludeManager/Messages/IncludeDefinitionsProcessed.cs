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

namespace PlcNext.CppParser.IncludeManager.Messages
{
    internal class IncludeDefinitionsProcessed : MessageDecorator
    {
        private IncludeDefinitionsProcessed(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        public static IncludeDefinitionsProcessed Decorate(CacheUpdated decoratedMessage,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new IncludeDefinitionsProcessed(decoratedMessage, additionalPredecessors);
        }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
