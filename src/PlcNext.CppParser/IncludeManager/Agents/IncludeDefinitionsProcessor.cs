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
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(InitialIncludeDefinitionsDefined))]
    [Consumes(typeof(CacheParsed))]
    [Consumes(typeof(IncludeFileProcessed))]
    [Consumes(typeof(IncludeFileProcessUnnecessary))]
    [Produces(typeof(IncludeDefinitionProcessing))]
    [Produces(typeof(IncludeDefinitionsProcessed))]
    internal class IncludeDefinitionsProcessor : Agent
    {
        private readonly SynchronizedCollection<IncludeDefinitionProcessing> openProcesses =
            new SynchronizedCollection<IncludeDefinitionProcessing>();
        private readonly MessageCollector<CacheParsed, InitialIncludeDefinitionsDefined> initialCollector;
        private readonly MessageCollector<CacheParsed, IncludeFileProcessed> processedCollector;
        private readonly MessageCollector<CacheParsed, IncludeFileProcessUnnecessary> unnecessaryCollector;
        
        public IncludeDefinitionsProcessor(IMessageBoard messageBoard) : base(messageBoard)
        {
            initialCollector = new MessageCollector<CacheParsed, InitialIncludeDefinitionsDefined>(OnInitialMessagesCollected);
            processedCollector = new MessageCollector<CacheParsed, IncludeFileProcessed>(OnProcessedMessagesCollected);
            unnecessaryCollector = new MessageCollector<CacheParsed, IncludeFileProcessUnnecessary>(OnUnnecessaryMessagesCollected);
        }

        private void OnInitialMessagesCollected(MessageCollection<CacheParsed, InitialIncludeDefinitionsDefined> set)
        {
            set.MarkAsConsumed(set.Message2);
            if (!set.Message2.Includes.Any())
            {
                OnMessage(IncludeDefinitionsProcessed.Decorate(CacheUpdated.Decorate(new CacheParsed(set, set.Message1.Cache))));
            }
            Start(set.Message2.Includes, set);
        }

        private void OnUnnecessaryMessagesCollected(MessageCollection<CacheParsed, IncludeFileProcessUnnecessary> set)
        {
            set.MarkAsConsumed(set.Message2);
            CheckForCompletion(set.Message2.MessageDomain, set.Message1);
        }

        private void OnProcessedMessagesCollected(MessageCollection<CacheParsed, IncludeFileProcessed> set)
        {
            set.MarkAsConsumed(set.Message2);
            Start(set.Message2.ChildDefinitions, set);
            CheckForCompletion(set.Message2.MessageDomain, set.Message1);
        }

        protected override void ExecuteCore(Message messageData)
        {
            processedCollector.TryPush(messageData);
            unnecessaryCollector.TryPush(messageData);
            initialCollector.TryPush(messageData);
        }

        private void CheckForCompletion(MessageDomain messageDomain, CacheParsed cacheParsed)
        {
            if (!messageDomain.Root.TryGet(out IncludeDefinitionProcessing definitionProcessing))
            {
                return;
            }
            if (openProcesses.Remove(definitionProcessing) && openProcesses.Count == 0)
            {
                OnMessage(IncludeDefinitionsProcessed.Decorate(CacheUpdated.Decorate(new CacheParsed(definitionProcessing, cacheParsed.Cache))));
            }
        }

        private void Start(IReadOnlyCollection<IncludeDefinition> resultIncludes, IEnumerable<Message> predecessors)
        {
            IncludeDefinitionProcessing[] processingMessages = resultIncludes.Select(i => new IncludeDefinitionProcessing(predecessors, i))
                                                                             .ToArray();
            foreach (IncludeDefinitionProcessing processingMessage in processingMessages)
            {
                openProcesses.Add(processingMessage);
            }

            OnMessages(processingMessages);
        }
    }
}
