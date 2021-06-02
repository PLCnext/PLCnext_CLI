#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Concurrent;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(IncludeDefinitionsProcessed))]
    [Produces(typeof(IncludeCacheUpdateParsingErrors))]
    [Intercepts(typeof(IncludeCacheEntryCreated))]
    internal class AddedEntryParsingErrorCollector : InterceptorAgent
    {
        private readonly ConcurrentBag<CodeSpecificException> parsingErrors = new ConcurrentBag<CodeSpecificException>();
        public AddedEntryParsingErrorCollector(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new IncludeCacheUpdateParsingErrors(messageData, parsingErrors));
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            IncludeCacheEntryCreated entryCreated = messageData.Get<IncludeCacheEntryCreated>();
            foreach (CodeSpecificException parsingError in entryCreated.ParsingErrors)
            {
                parsingErrors.Add(parsingError);
            }

            return InterceptionAction.Continue;
        }
    }
}
