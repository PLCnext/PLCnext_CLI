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
    public class StructureCreated : MessageDecorator
    {
        private StructureCreated(Message decoratedMessage, IStructure structure, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            Structure = structure;
        }

        public static StructureCreated Decorate(TypeCreated decoratedMessage, IStructure structure,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new StructureCreated(decoratedMessage, structure, additionalPredecessors);
        }
        
        public IStructure Structure { get; }

        protected override string DataToString()
        {
            return $"{nameof(Structure)}: {Structure}";
        }
    }
}
