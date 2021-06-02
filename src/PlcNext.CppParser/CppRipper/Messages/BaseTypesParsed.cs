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
    internal class BaseTypesParsed : Message
    {
        public BaseTypesParsed(Message predecessorMessage, CppDataType[] baseTypes): base(predecessorMessage)
        {
            BaseTypes = baseTypes;
        }

        public BaseTypesParsed(IEnumerable<Message> predecessorMessages, CppDataType[] baseTypes): base(predecessorMessages)
        {
            BaseTypes = baseTypes;
        }
		
        public CppDataType[] BaseTypes { get; }
		
        protected override string DataToString()
        {
            return $"{nameof(BaseTypes)}: {BaseTypes}";
        }
    }
}
