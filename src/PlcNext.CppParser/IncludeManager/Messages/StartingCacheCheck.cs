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
    internal class StartingCacheCheck : Message
    {
        public StartingCacheCheck(Message predecessorMessage, IIncludeCache cache): base(predecessorMessage)
        {
            Cache = cache;
        }

        public StartingCacheCheck(IEnumerable<Message> predecessorMessages, IIncludeCache cache): base(predecessorMessages)
        {
            Cache = cache;
        }
        
        public IIncludeCache Cache { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
