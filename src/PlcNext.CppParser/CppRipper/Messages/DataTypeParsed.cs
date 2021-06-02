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
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class DataTypeParsed : Message
    {
        public DataTypeParsed(Message predecessorMessage, IReadOnlyCollection<ParseNode> typeNodes, ParseNode declaration, CppDataType dataType): base(predecessorMessage)
        {
            TypeNodes = typeNodes;
            Declaration = declaration;
            DataType = dataType;
        }

        public DataTypeParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<ParseNode> typeNodes, ParseNode declaration, CppDataType dataType): base(predecessorMessages)
        {
            TypeNodes = typeNodes;
            Declaration = declaration;
            DataType = dataType;
        }
        
        public IReadOnlyCollection<ParseNode> TypeNodes { get; }

        public ParseNode Declaration { get; }
        
        public CppDataType DataType { get; }

        protected override string DataToString()
        {
            return $"{nameof(DataType)}: {DataType}";
        }
    }
}
