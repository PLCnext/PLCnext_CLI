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
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(CppFileFound))]
    [Intercepts(typeof(DeclarationInvalid))]
    [Intercepts(typeof(CppFileResult))]
    internal class ParserMessageAggregator : InterceptorAgent
    {
        private readonly MessageCollector<CppFileFound, DeclarationInvalid> collector;

        private readonly ConcurrentDictionary<MessageDomain, ConcurrentBag<ParserMessage>> errorMessages =
            new ConcurrentDictionary<MessageDomain, ConcurrentBag<ParserMessage>>();
        public ParserMessageAggregator(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<CppFileFound, DeclarationInvalid>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<CppFileFound, DeclarationInvalid> set)
        {
            set.MarkAsConsumed(set.Message2);
            if (set.Message2.ErrorMessage != null)
            {
                ConcurrentBag<ParserMessage> messages =
                    errorMessages.GetOrAdd(set.Message1.MessageDomain, new ConcurrentBag<ParserMessage>());
                messages.Add(set.Message2.ErrorMessage);
            }
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.TryGet(out DeclarationInvalid declarationInvalid))
            {
                collector.Push(declarationInvalid);
                return InterceptionAction.Continue;
            }
            
            if (errorMessages.TryRemove(messageData.MessageDomain, out ConcurrentBag<ParserMessage> messages))
            {
                CppFileResult result = messageData.Get<CppFileResult>();
                result.AddErrorMessages(messages);
            }
            return InterceptionAction.Continue;
        }
    }
}
