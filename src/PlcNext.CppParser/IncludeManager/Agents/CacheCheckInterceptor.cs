#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Produces(typeof(StartingCacheCheck))]
    [Intercepts(typeof(CacheParsed))]
    internal class CacheCheckInterceptor : InterceptorAgent
    {
        public CacheCheckInterceptor(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.Is<CacheUpdated>())
            {
                //Already checked cache
                return InterceptionAction.Continue;
            }

            if (!messageData.Get<CacheParsed>()
                            .Cache.Entries.Any())
            {
                return InterceptionAction.Continue;
            }
            
            OnMessage(new StartingCacheCheck(messageData, messageData.Get<CacheParsed>().Cache));
            return InterceptionAction.DoNotPublish;
        }
    }
}