#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(StartingCacheCheck))]
    [Produces(typeof(CheckingCacheEntry))]
    internal class CacheCheckStarter : Agent
    {
        public CacheCheckStarter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CheckingCacheEntry[] checkMessages = messageData.Get<StartingCacheCheck>()
                                                            .Cache.Entries
                                                            .Select(e => new CheckingCacheEntry(messageData, e))
                                                            .ToArray();
            OnMessages(checkMessages);
        }
    }
}
