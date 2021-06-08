#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using Agents.Net;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(CodeModelCreationParameters))]
    [Consumes(typeof(ExceptionMessage))]
    [Produces(typeof(ExceptionCancellationTokenCreated))]
    internal class ExceptionCancelExecuter : Agent
    {
        private readonly MessageCollector<ExceptionCancellationTokenCreated, ExceptionMessage> collector;
        public ExceptionCancelExecuter(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<ExceptionCancellationTokenCreated, ExceptionMessage>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<ExceptionCancellationTokenCreated, ExceptionMessage> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            
            set.Message1.CancellationTokenSource.Cancel();
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData.Is<CodeModelCreationParameters>())
            {
                CancellationTokenSource tokenSource = new();
                AddDisposable(tokenSource);
                ExceptionCancellationTokenCreated tokenCreated = new(messageData, tokenSource, tokenSource.Token);
                collector.Push(tokenCreated);
                OnMessage(tokenCreated);
            }
            else
            {
                collector.Push(messageData);
            }
        }
    }
}
