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
    public class ClassCreated : MessageDecorator
    {
        private ClassCreated(Message decoratedMessage, IClass @class, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            Class = @class;
        }

        public static ClassCreated Decorate(TypeCreated decoratedMessage, IClass @class,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new ClassCreated(decoratedMessage, @class, additionalPredecessors);
        }
        
        public IClass Class { get; }

        protected override string DataToString()
        {
            return $"{nameof(Class)}: {Class}";
        }
    }
}
