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
    internal class IncludeDefinitionProcessing : Message
    {
        public IncludeDefinitionProcessing(Message predecessorMessage, IncludeDefinition includeDefinition): base(predecessorMessage)
        {
            IncludeDefinition = includeDefinition;
        }

        public IncludeDefinitionProcessing(IEnumerable<Message> predecessorMessages, IncludeDefinition includeDefinition): base(predecessorMessages)
        {
            IncludeDefinition = includeDefinition;
        }
        
        public IncludeDefinition IncludeDefinition { get; }

        protected override string DataToString()
        {
            return $"{nameof(IncludeDefinition)}: {IncludeDefinition}";
        }
    }
}
