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
    public class NameParsed : Message
    {
        public NameParsed(Message predecessorMessage, string name, string ns): base(predecessorMessage)
        {
            Name = name;
            Namespace = ns;
        }

        public NameParsed(IEnumerable<Message> predecessorMessages, string name, string ns): base(predecessorMessages)
        {
            Name = name;
            Namespace = ns;
        }
        
        public string Name { get; }
        public string Namespace { get; }

        protected override string DataToString()
        {
            return $"{nameof(Name)}: {Name}; {nameof(Namespace)}:{Namespace}";
        }
    }
}
