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
    internal class NoSourceDirectoriesFound : MessageDecorator
    {
        private NoSourceDirectoriesFound(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }

        public static NoSourceDirectoriesFound Create(Message predecessor)
        {
            return new NoSourceDirectoriesFound(new SourceDirectoriesParsed(predecessor, Array.Empty<FileResult>()),
                                                new[] {predecessor});
        }
    }
}
