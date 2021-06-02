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
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(CppFileResult))]
    [Produces(typeof(CppFilesParsed))]
    public class CppFileAggregator : Agent
    {
        private MessageAggregator<CppFileResult> aggregator;
        public CppFileAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<CppFileResult>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<CppFileResult> set)
        {
            OnMessage(new CppFilesParsed(set, set.Select(p => p.FileResult).ToArray()));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
