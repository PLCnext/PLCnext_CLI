#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Agents.Net;

namespace PlcNext.CppParser.Messages
{
    public class DefineStatementsParsed : Message
    {
        public DefineStatementsParsed(Message predecessorMessage, Dictionary<string, string> defineStatements): base(predecessorMessage)
        {
            DefineStatements = defineStatements;
        }

        public DefineStatementsParsed(IEnumerable<Message> predecessorMessages, Dictionary<string, string> defineStatements): base(predecessorMessages)
        {
            DefineStatements = defineStatements;
        }
        
        public Dictionary<string, string> DefineStatements { get; }

        protected override string DataToString()
        {
            return $"{nameof(DefineStatements)}: {string.Join("; ", DefineStatements.Select(kv => $"{kv.Key}: {kv.Value}"))}";
        }
    }
}
