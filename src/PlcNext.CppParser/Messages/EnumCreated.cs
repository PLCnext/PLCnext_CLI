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
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.Messages
{
    public class EnumCreated : MessageDecorator
    {
        private EnumCreated(Message decoratedMessage, IEnum @enum, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            Enum = @enum;
        }

        public static EnumCreated Decorate(TypeCreated decoratedMessage, IEnum @enum,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new EnumCreated(decoratedMessage, @enum, additionalPredecessors);
        }

        public IEnum Enum { get; }

        protected override string DataToString()
        {
            return $"{nameof(Enum)}: {Enum}";
        }
    }
}
