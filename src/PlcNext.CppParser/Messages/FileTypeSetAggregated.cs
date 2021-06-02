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
    public class FileTypeSetAggregated : Message
    {
        public FileTypeSetAggregated(Message predecessorMessage, Dictionary<IType, CodePosition> types): base(predecessorMessage)
        {
            Types = types;
        }

        public FileTypeSetAggregated(IEnumerable<Message> predecessorMessages, Dictionary<IType, CodePosition> types): base(predecessorMessages)
        {
            Types = types;
        }
        
        public Dictionary<IType, CodePosition> Types { get; }

        protected override string DataToString()
        {
            return $"{nameof(Types)}: {string.Join(", ", Types)}";
        }
    }
}
