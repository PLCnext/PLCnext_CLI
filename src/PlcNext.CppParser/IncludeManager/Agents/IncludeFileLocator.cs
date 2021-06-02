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
using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;
using Serilog;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(IncludeDefinitionProcessing))]
    [Consumes(typeof(IncludeDirectoriesFound))]
    [Consumes(typeof(CacheParsed))]
    [Produces(typeof(IncludeFileFound))]
    [Produces(typeof(IncludeFileProcessUnnecessary))]
    public class IncludeFileLocator : Agent
    {
        private readonly MessageCollector<IncludeDefinitionProcessing, IncludeDirectoriesFound, CacheParsed> collector;
        public IncludeFileLocator(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<IncludeDefinitionProcessing, IncludeDirectoriesFound, CacheParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<IncludeDefinitionProcessing, IncludeDirectoriesFound, CacheParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            
            if (set.Message1.IncludeDefinition.Include.Trim().StartsWith("<", StringComparison.Ordinal))
            {
                OnMessage(new IncludeFileProcessUnnecessary(set));
                return;
            }
            
            //Try parse relative to source
            if (set.Message1.IncludeDefinition.DefinitionSourceFile.Parent.TryGetFileFromPath(
                set.Message1.IncludeDefinition.Include, out VirtualFile file))
            {
                IncludeFindResult result = set.Message3.Cache.TryGetCacheEntry(file.FullName, out IncludeCacheEntry cacheEntry)
                                              ? new IncludeFindResult(cacheEntry)
                                              : new IncludeFindResult(file, set.Message1.IncludeDefinition.DefinitionSourceBaseDirectory);
                OnMessage(new IncludeFileFound(set, result));
                return;
            }
            
            //Parse relative to include path
            foreach (VirtualDirectory includeDirectory in set.Message2.IncludeDirectories.Select(x => x.Directory).Where(v => v != null))
            {
                if (includeDirectory.TryGetFileFromPath(set.Message1.IncludeDefinition.Include, out file))
                {
                    IncludeFindResult result = set.Message3.Cache.TryGetCacheEntry(file.FullName, out IncludeCacheEntry cacheEntry)
                                                   ? new IncludeFindResult(cacheEntry)
                                                   : new IncludeFindResult(file, includeDirectory);
                    OnMessage(new IncludeFileFound(set, result));
                    return;
                }
            }
            
            Log.Warning($"Could not find include {set.Message1.IncludeDefinition.Include}. Possible types from these files will not be parsed.");
            OnMessage(new IncludeFileProcessUnnecessary(set));
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
