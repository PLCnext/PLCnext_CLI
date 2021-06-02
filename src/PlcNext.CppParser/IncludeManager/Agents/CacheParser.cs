#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CacheFileOpened))]
    [Produces(typeof(CacheParsed))]
    internal class CacheParser : Agent
    {
        public CacheParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CacheFileOpened fileOpened = messageData.Get<CacheFileOpened>();
            IIncludeCache cache = JsonIncludeCache.LoadCache(fileOpened.CacheFileStream);
            OnMessage(new CacheParsed(messageData, cache));
        }
    }
}
