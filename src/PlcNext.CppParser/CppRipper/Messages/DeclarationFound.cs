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
    public class DeclarationFound : Message
    {
        public DeclarationFound(Message predecessorMessage, ParseNode declaration, int fileIndex): base(predecessorMessage)
        {
            Declaration = declaration;
            FileIndex = fileIndex;
        }

        public DeclarationFound(IEnumerable<Message> predecessorMessages, ParseNode declaration, int fileIndex): base(predecessorMessages)
        {
            Declaration = declaration;
            FileIndex = fileIndex;
        }

        public ParseNode Declaration { get; }
        
        public int FileIndex { get; }
        
        protected override string DataToString()
        {
            return $"{nameof(Declaration)}: {Declaration}; {nameof(FileIndex)}: {FileIndex}";
        }
    }
}
