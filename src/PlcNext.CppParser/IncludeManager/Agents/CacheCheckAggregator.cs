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
    [Consumes(typeof(CacheEntryChecked))]
    [Produces(typeof(CacheCheckResult))]
    public class CacheCheckAggregator : Agent
    {
        private readonly MessageAggregator<CacheEntryChecked> aggregator;
        public CacheCheckAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<CacheEntryChecked>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<CacheEntryChecked> set)
        {
            OnMessage(new CacheCheckResult(set, set.ToDictionary(m => m.Entry, m => m.CheckResult)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
