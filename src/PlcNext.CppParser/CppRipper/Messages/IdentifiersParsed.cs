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
    public class IdentifiersParsed : Message
    {
        public IdentifiersParsed(Message predecessorMessage, IReadOnlyCollection<ParseNode> identifiers): base(predecessorMessage)
        {
            Identifiers = identifiers;
        }

        public IdentifiersParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<ParseNode> identifiers): base(predecessorMessages)
        {
            Identifiers = identifiers;
        }

        public IReadOnlyCollection<ParseNode> Identifiers { get; }

        protected override string DataToString()
        {
            return $"{nameof(Identifiers)}: {string.Join<ParseNode>(", ", Identifiers)}";
        }
    }
}
