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

namespace PlcNext.CppParser.Messages
{
    internal class CppFilesParsed : Message
    {
        public CppFilesParsed(Message predecessorMessage, IReadOnlyCollection<FileResult> results): base(predecessorMessage)
        {
            Results = results;
        }

        public CppFilesParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<FileResult> results): base(predecessorMessages)
        {
            Results = results;
        }
        
        public IReadOnlyCollection<FileResult> Results { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
