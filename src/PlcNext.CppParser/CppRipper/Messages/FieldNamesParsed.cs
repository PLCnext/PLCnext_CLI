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
    internal class FieldNamesParsed : Message
    {
        public FieldNamesParsed(Message predecessorMessage, string[] fieldNames): base(predecessorMessage)
        {
            FieldNames = fieldNames;
        }

        public FieldNamesParsed(IEnumerable<Message> predecessorMessages, string[] fieldNames): base(predecessorMessages)
        {
            FieldNames = fieldNames;
        }
        
        public string[] FieldNames { get; }

        protected override string DataToString()
        {
            return $"{nameof(FieldNames)}: {FieldNames}";
        }
    }
}
