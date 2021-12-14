#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(InitialIncludeDefinitionsDefined))]
    [Produces(typeof(IncludeCacheEntryCreated))]
    [Intercepts(typeof(CppFileResult))]
    //This is the finisher for parsing new files to add to the cache
    internal sealed class IncludeFileResultEvaluater : InterceptorAgent
    {
        private readonly MessageCollector<InitialIncludeDefinitionsDefined, CppFileResult> collector = new MessageCollector<InitialIncludeDefinitionsDefined, CppFileResult>();
        private readonly object codeModelLock = new();
        public IncludeFileResultEvaluater(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (!messageData.MessageDomain.Root.Is<IncludeCppFileFound>())
            {
                return InterceptionAction.Continue;
            }
            
            collector.PushAndExecute(messageData, set =>
            {
                set.MarkAsConsumed(set.Message2);
                MessageDomain.TerminateDomainsOf(set.Message2);
                lock (codeModelLock)
                {
                    foreach (IType type in set.Message2.FileResult.Types.Keys)
                    {
                        set.Message1.CodeModel.AddType(type, set.Message2.FileResult.File, set.Message2.FileResult.RootDirectory);
                    }
                }

                IncludeCacheEntry cacheEntry = new IncludeCacheEntry(set.Message2.FileResult.File.FullName,
                                                                     set.Message2.FileResult.ParsedSuccessfully,
                                                                     set.Message2.FileResult.File.LastWriteTime,
                                                                     set.Message2.FileResult.RootDirectory.FullName,
                                                                     set.Message2.FileResult.Types.Keys.Select(
                                                                         t => t.FullName),
                                                                     set.Message2.FileResult.Includes,
                                                                     set.Message2.FileResult.DefineStatements,
                                                                     new Dictionary<IConstant, CodePosition>());
                string relativePath = set.Message2.FileResult.File.GetRelativePath(set.Message2.FileResult.RootDirectory);
                IncludeDefinition[] childIncludes = set.Message2.FileResult.Includes.Select(
                                                                   i => new IncludeDefinition(i, set.Message2.FileResult.File, set.Message2.FileResult.RootDirectory))
                                                              .ToArray();
                CodeSpecificException[] parsingErrors = set.Message2
                                                       .FileResult.Errors
                                                       .Select(e => e.ToException(set.Message2.FileResult.File))
                                                       .ToArray();
                OnMessage(IncludeCacheEntryCreated.Decorate(new IncludeFileProcessed(messageData, cacheEntry, childIncludes, relativePath), cacheEntry, parsingErrors)); 
            });
            
            return InterceptionAction.DoNotPublish;
        }
    }
}
