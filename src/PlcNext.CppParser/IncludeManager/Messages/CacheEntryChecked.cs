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
    internal class CacheEntryChecked : Message
    {
        public CacheEntryChecked(Message predecessorMessage, IncludeCacheEntry entry, CheckResult checkResult): base(predecessorMessage)
        {
            Entry = entry;
            CheckResult = checkResult;
        }

        public CacheEntryChecked(IEnumerable<Message> predecessorMessages, IncludeCacheEntry entry, CheckResult checkResult): base(predecessorMessages)
        {
            Entry = entry;
            CheckResult = checkResult;
        }
        
        public IncludeCacheEntry Entry { get; }

        public CheckResult CheckResult { get; }
        
        protected override string DataToString()
        {
            return $"{nameof(Entry)}: {Entry}; {nameof(CheckResult)}: {CheckResult}";
        }
    }

    internal enum CheckResult
    {
        None,
        Outdated,
        Missing
    }
}
