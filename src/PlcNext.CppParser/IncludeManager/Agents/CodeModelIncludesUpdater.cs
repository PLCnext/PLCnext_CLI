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
    [Consumes(typeof(IncludesRegisteredToCodeModel))]
    [Produces(typeof(RegisteringIncludesToCodeModel))]
    [Intercepts(typeof(CodeModelCreated))]
    internal class CodeModelIncludesUpdater : InterceptorAgent
    {
        private readonly MessageCollector<RegisteringIncludesToCodeModel, IncludesRegisteredToCodeModel> collector =
            new MessageCollector<RegisteringIncludesToCodeModel, IncludesRegisteredToCodeModel>();
        public CodeModelIncludesUpdater(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            CodeModelCreated codeModelCreated = messageData.Get<CodeModelCreated>();
            RegisteringIncludesToCodeModel message =
                new RegisteringIncludesToCodeModel(messageData, codeModelCreated.CodeModel);
            OnMessage(message);
            collector.PushAndExecute(message, set => {});

            return InterceptionAction.Continue;
        }
    }
}
