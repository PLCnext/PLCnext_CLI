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
    [Consumes(typeof(TypeCreated))]
    [Produces(typeof(FileTypeSetAggregated))]
    public class FileTypesAggregator : Agent
    {
        private readonly MessageAggregator<TypeCreated> aggregator;
        public FileTypesAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            aggregator = new MessageAggregator<TypeCreated>(OnAggregated);
        }

        private void OnAggregated(IReadOnlyCollection<TypeCreated> set)
        {
            OnMessage(new FileTypeSetAggregated(set, set.OrderBy(t => t.CodePosition.Line).ToDictionary(t => t.Type, t => t.CodePosition)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            aggregator.Aggregate(messageData);
        }
    }
}
