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
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class NoDeclarationFound : MessageDecorator
    {
        private NoDeclarationFound(Message decoratedMessage, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
        }

        protected override string DataToString()
        {
            return string.Empty;
        }

        public static NoDeclarationFound Create(Message predecessor)
        {
            return new NoDeclarationFound(new TypeFieldsAggregated(predecessor, Array.Empty<CppField>()),
                                          new[] {predecessor});
        }
    }
}
