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

namespace PlcNext.CppParser.Messages
{
    public class AttributePrefixDefined : Message
    {
        public AttributePrefixDefined(Message predecessorMessage, string attributePrefix): base(predecessorMessage)
        {
            AttributePrefix = attributePrefix;
        }

        public AttributePrefixDefined(IEnumerable<Message> predecessorMessages, string attributePrefix): base(predecessorMessages)
        {
            AttributePrefix = attributePrefix;
        }
        
        public string AttributePrefix { get; }

        protected override string DataToString()
        {
            return $"{nameof(AttributePrefix)}: {AttributePrefix}";
        }
    }
}
