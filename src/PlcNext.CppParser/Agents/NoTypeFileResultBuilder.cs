#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(IncludesFound))]
    [Consumes(typeof(NoTypeDeclarationFound))]
    [Consumes(typeof(CppFileFound))]
    [Consumes(typeof(DefineStatementsParsed))]
    [Produces(typeof(CppFileResult))]
    public class NoTypeFileResultBuilder : Agent
    {
        private readonly MessageCollector<IncludesFound, NoTypeDeclarationFound, CppFileFound, DefineStatementsParsed> collector;
        public NoTypeFileResultBuilder(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<IncludesFound, NoTypeDeclarationFound, CppFileFound, DefineStatementsParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<IncludesFound, NoTypeDeclarationFound, CppFileFound, DefineStatementsParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);
            set.MarkAsConsumed(set.Message4);
            
            OnMessage(new CppFileResult(set, new FileResult(new Dictionary<IType, CodePosition>(), set.Message1.Includes, set.Message4.DefineStatements, set.Message3.File, set.Message3.Directory)));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
