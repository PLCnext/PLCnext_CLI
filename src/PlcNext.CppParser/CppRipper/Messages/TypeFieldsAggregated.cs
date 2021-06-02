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
    internal class TypeFieldsAggregated : Message
    {
        public TypeFieldsAggregated(Message predecessorMessage, IReadOnlyCollection<CppField> fields): base(predecessorMessage)
        {
            Fields = fields;
        }

        public TypeFieldsAggregated(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CppField> fields): base(predecessorMessages)
        {
            Fields = fields;
        }

        public IReadOnlyCollection<CppField> Fields { get; }

        protected override string DataToString()
        {
            return $"{nameof(Fields)}: {Fields}";
        }
    }
}
