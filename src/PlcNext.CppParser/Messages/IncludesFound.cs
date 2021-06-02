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
    public class IncludesFound : Message
    {
        public IncludesFound(Message predecessorMessage, IReadOnlyCollection<string> includes): base(predecessorMessage)
        {
            Includes = includes;
        }

        public IncludesFound(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<string> includes): base(predecessorMessages)
        {
            Includes = includes;
        }
        
        public IReadOnlyCollection<string> Includes { get; }

        protected override string DataToString()
        {
            return $"{nameof(Includes)}: {Includes}";
        }
    }
}
