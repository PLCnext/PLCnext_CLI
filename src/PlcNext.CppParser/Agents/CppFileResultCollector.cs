#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(FileTypeSetAggregated))]
    [Consumes(typeof(IncludesFound))]
    [Consumes(typeof(CppFileFound))]
    [Consumes(typeof(DefineStatementsParsed))]
    [Produces(typeof(CppFileResult))]
    public class  CppFileResultCollector : Agent
    {
        private readonly MessageCollector<FileTypeSetAggregated, IncludesFound, CppFileFound, DefineStatementsParsed> collector;
        public CppFileResultCollector(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<FileTypeSetAggregated, IncludesFound, CppFileFound, DefineStatementsParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<FileTypeSetAggregated, IncludesFound, CppFileFound, DefineStatementsParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);
            set.MarkAsConsumed(set.Message4);
            OnMessage(new CppFileResult(set, new FileResult(set.Message1.Types, set.Message2.Includes, set.Message4.DefineStatements, set.Message3.File, set.Message3.Directory)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
