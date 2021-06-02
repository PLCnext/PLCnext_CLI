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
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(FieldSetConstructed))]
    [Produces(typeof(TypeFieldsAggregated))]
    public class TypeFieldAggregator : Agent
    {
        private readonly MessageAggregator<FieldSetConstructed> aggregator;
        public TypeFieldAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<FieldSetConstructed>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<FieldSetConstructed> set)
        {
            OnMessage(new TypeFieldsAggregated(set, set.OrderBy(m => m.FileIndex).SelectMany(m => m.FieldSet).ToArray()));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
