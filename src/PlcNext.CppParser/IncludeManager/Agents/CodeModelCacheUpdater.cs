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
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(IncludeDefinitionsProcessed))]
    [Produces(typeof(InitialIncludeDefinitionsDefined))]
    [Intercepts(typeof(CodeModelCreated))]
    internal class CodeModelCacheUpdater : InterceptorAgent
    {
        private readonly MessageCollector<IncludeDefinitionsProcessed, InitialIncludeDefinitionsDefined> collector = new MessageCollector<IncludeDefinitionsProcessed, InitialIncludeDefinitionsDefined>();
        public CodeModelCacheUpdater(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            CodeModelCreated created = messageData.Get<CodeModelCreated>();
            InitialIncludeDefinitionsDefined defined =
                new InitialIncludeDefinitionsDefined(messageData, created.Includes, created.ParsedFiles,
                                                     created.CodeModel);
            OnMessage(defined);
            collector.PushAndExecute(defined, set => {});
            //just wait for finished execution before continue sending code model created message
            return InterceptionAction.Continue;
        }
    }
}
