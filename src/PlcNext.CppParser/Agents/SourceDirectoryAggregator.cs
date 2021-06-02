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
    [Consumes(typeof(CppFilesParsed))]
    [Produces(typeof(SourceDirectoriesParsed))]
    public class SourceDirectoryAggregator : Agent
    {
        private readonly MessageAggregator<CppFilesParsed> aggregator;
        public SourceDirectoryAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<CppFilesParsed>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<CppFilesParsed> set)
        {
            OnMessage(new SourceDirectoriesParsed(set, set.SelectMany(s => s.Results).ToArray()));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
