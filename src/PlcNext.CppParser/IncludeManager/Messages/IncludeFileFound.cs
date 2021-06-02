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

namespace PlcNext.CppParser.IncludeManager.Messages
{
    internal class IncludeFileFound : Message
    {
        public IncludeFileFound(Message predecessorMessage, IncludeFindResult findResult): base(predecessorMessage)
        {
            FindResult = findResult;
        }

        public IncludeFileFound(IEnumerable<Message> predecessorMessages, IncludeFindResult findResult): base(predecessorMessages)
        {
            FindResult = findResult;
        }
        
        public IncludeFindResult FindResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(FindResult)}: {FindResult}";
        }
    }
}
