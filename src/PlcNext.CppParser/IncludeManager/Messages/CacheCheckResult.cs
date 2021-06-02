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
    internal class CacheCheckResult : Message
    {
        public CacheCheckResult(Message predecessorMessage, Dictionary<IncludeCacheEntry, CheckResult> results): base(predecessorMessage)
        {
            Results = results;
        }

        public CacheCheckResult(IEnumerable<Message> predecessorMessages, Dictionary<IncludeCacheEntry, CheckResult> results): base(predecessorMessages)
        {
            Results = results;
        }
        
        public Dictionary<IncludeCacheEntry, CheckResult> Results { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
