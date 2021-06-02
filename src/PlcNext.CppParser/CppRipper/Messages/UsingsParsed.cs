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

namespace PlcNext.CppParser.CppRipper.Messages
{
    public class UsingsParsed : Message
    {
        public UsingsParsed(Message predecessorMessage, IReadOnlyCollection<string> usings): base(predecessorMessage)
        {
            Usings = usings;
        }

        public UsingsParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<string> usings): base(predecessorMessages)
        {
            Usings = usings;
        }
        
        public IReadOnlyCollection<string> Usings { get; }

        protected override string DataToString()
        {
            return $"{nameof(Usings)}: {string.Join(", ", Usings)}";
        }
    }
}
