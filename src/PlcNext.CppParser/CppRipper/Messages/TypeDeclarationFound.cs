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
    public class TypeDeclarationFound : Message
    {
        public TypeDeclarationFound(Message predecessorMessage, ParseNode declaration, ParseNode content): base(predecessorMessage)
        {
            Declaration = declaration;
            Content = content;
        }

        public TypeDeclarationFound(IEnumerable<Message> predecessorMessages, ParseNode declaration, ParseNode content): base(predecessorMessages)
        {
            Declaration = declaration;
            Content = content;
        }
        
        public ParseNode Content { get; }
        public ParseNode Declaration { get; }

        protected override string DataToString()
        {
            return $"{nameof(Declaration)}: {Declaration}";
        }
    }
}
