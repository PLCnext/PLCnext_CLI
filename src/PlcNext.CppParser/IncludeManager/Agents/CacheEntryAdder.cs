#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Concurrent;
using System.Threading;
using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Intercepts(typeof(IncludeCacheEntryCreated))]
    [Intercepts(typeof(IncludeDefinitionsProcessed))]
    internal sealed class CacheEntryAdder : InterceptorAgent
    {
        private readonly ConcurrentBag<IncludeCacheEntry> addedEntries = new ConcurrentBag<IncludeCacheEntry>();
        public CacheEntryAdder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }
		
        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.TryGet(out CacheParsed cacheParsed))
            {
                cacheParsed.Cache.AddEntries(addedEntries);
            }
            else
            {
                addedEntries.Add(messageData.Get<IncludeCacheEntryCreated>().CreatedEntry);
            }

            return InterceptionAction.Continue;
        }
    }
}
