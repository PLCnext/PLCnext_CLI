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
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CacheCheckResult))]
    [Consumes(typeof(StartingCacheCheck))]
    [Produces(typeof(CacheUpdated))]
    public class CacheCheckUpdater : Agent
    {
        private readonly MessageCollector<CacheCheckResult, StartingCacheCheck> collector;
        public CacheCheckUpdater(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<CacheCheckResult, StartingCacheCheck>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<CacheCheckResult, StartingCacheCheck> set)
        {
            set.MarkAsConsumed(set.Message1);
            
            IEnumerable<IncludeCacheEntry> deletableEntries = set.Message1.Results.Where(kv => kv.Value != CheckResult.None).Select(kv => kv.Key);
            set.Message2.Cache.DeleteEntries(deletableEntries);
            OnMessage(CacheUpdated.Decorate(new CacheParsed(set, set.Message2.Cache)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
