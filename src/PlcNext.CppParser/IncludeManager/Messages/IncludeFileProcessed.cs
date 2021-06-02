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

namespace PlcNext.CppParser.IncludeManager.Messages
{
    internal class IncludeFileProcessed : Message
    {
        public IncludeFileProcessed(Message predecessorMessage, IncludeCacheEntry cacheEntry, IReadOnlyCollection<IncludeDefinition> childDefinitions, string relativePath): base(predecessorMessage)
        {
            CacheEntry = cacheEntry;
            ChildDefinitions = childDefinitions;
            RelativePath = relativePath;
        }

        public IncludeFileProcessed(IEnumerable<Message> predecessorMessages, IncludeCacheEntry cacheEntry, IReadOnlyCollection<IncludeDefinition> childDefinitions, string relativePath): base(predecessorMessages)
        {
            CacheEntry = cacheEntry;
            ChildDefinitions = childDefinitions;
            RelativePath = relativePath;
        }
        
        public IncludeCacheEntry CacheEntry { get; }
        
        public IReadOnlyCollection<IncludeDefinition> ChildDefinitions { get; }
        
        public string RelativePath { get; }

        protected override string DataToString()
        {
            return $"{nameof(CacheEntry)}: {CacheEntry}; {nameof(RelativePath)}: {RelativePath}; {nameof(ChildDefinitions)}: {string.Join(", ", ChildDefinitions)}";
        }
    }
}
