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
    internal class CheckingCacheEntry : Message
    {
        public CheckingCacheEntry(Message predecessorMessage, IncludeCacheEntry entryToCheck): base(predecessorMessage)
        {
            EntryToCheck = entryToCheck;
        }

        public CheckingCacheEntry(IEnumerable<Message> predecessorMessages, IncludeCacheEntry entryToCheck): base(predecessorMessages)
        {
            EntryToCheck = entryToCheck;
        }

        public IncludeCacheEntry EntryToCheck { get; }

        protected override string DataToString()
        {
            return $"{nameof(EntryToCheck)}:{EntryToCheck}";
        }
    }
}
