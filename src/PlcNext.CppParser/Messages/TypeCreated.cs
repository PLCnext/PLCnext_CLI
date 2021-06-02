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
    public class TypeCreated : Message
    {
        public TypeCreated(Message predecessorMessage, IType type, CodePosition codePosition): base(predecessorMessage)
        {
            Type = type;
            CodePosition = codePosition;
        }

        public TypeCreated(IEnumerable<Message> predecessorMessages, IType type, CodePosition codePosition): base(predecessorMessages)
        {
            Type = type;
            CodePosition = codePosition;
        }
        
        public IType Type { get; }
        
        public CodePosition CodePosition { get; }

        protected override string DataToString()
        {
            return $"{nameof(Type)}: {Type}; {nameof(CodePosition)}: {CodePosition}";
        }
    }
}
