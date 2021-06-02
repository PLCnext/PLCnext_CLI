#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using Agents.Net;

namespace PlcNext.CppParser.Messages
{
    internal class EmptyDirectoryFound : MessageDecorator
    {
        private EmptyDirectoryFound(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }

        public static EmptyDirectoryFound Create(Message predecessor)
        {
            return new EmptyDirectoryFound(new CppFilesParsed(predecessor, Array.Empty<FileResult>()),
                                           new[] {predecessor});
        }
    }
}
