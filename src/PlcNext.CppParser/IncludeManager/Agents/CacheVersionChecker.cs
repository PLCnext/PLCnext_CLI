#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Produces(typeof(CacheUpdated))]
    [Intercepts(typeof(StartingCacheCheck))]
    internal class CacheVersionChecker : InterceptorAgent
    {
        public CacheVersionChecker(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            StartingCacheCheck cacheCheck = messageData.Get<StartingCacheCheck>();
            if (cacheCheck.Cache.Version != JsonIncludeCache.CurrentVersion)
            {
                OnMessage(CacheUpdated.Decorate(new CacheParsed(messageData, JsonIncludeCache.Empty)));
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
