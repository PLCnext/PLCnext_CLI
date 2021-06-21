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
using System.Threading;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Intercepts(typeof(IncludeFileProcessed))]
    [Consumes(typeof(InitialIncludeDefinitionsDefined))]
    internal sealed class IncludeDefineStatementsRegister : InterceptorAgent
    {
        private readonly MessageCollector<InitialIncludeDefinitionsDefined, IncludeFileProcessed> collector = new();
        private readonly object codeModelLock = new();
        public IncludeDefineStatementsRegister(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            collector.PushAndExecute(messageData, set =>
            {
                set.MarkAsConsumed(set.Message2);
                lock (codeModelLock)
                {
                    set.Message1.CodeModel.AddDefineStatements(set.Message2.CacheEntry.DefineStatements);
                }
            });
            
            return InterceptionAction.Continue;
        }
    }
}
