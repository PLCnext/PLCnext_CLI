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
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    //This is the finisher for parsing new files to add to the cache
    internal class IncludeCacheEntryCreated : MessageDecorator
    {
        private IncludeCacheEntryCreated(Message decoratedMessage, IncludeCacheEntry createdEntry, IReadOnlyCollection<CodeSpecificException> parsingErrors, IEnumerable<Message> additionalPredecessors = null)
            : base(decoratedMessage, additionalPredecessors)
        {
            CreatedEntry = createdEntry;
            ParsingErrors = parsingErrors;
        }

        public static IncludeCacheEntryCreated Decorate(IncludeFileProcessed decoratedMessage, IncludeCacheEntry createdEntry, IReadOnlyCollection<CodeSpecificException> parsingErrors,
                                          IEnumerable<Message> additionalPredecessors = null)
        {
            return new IncludeCacheEntryCreated(decoratedMessage, createdEntry, parsingErrors, additionalPredecessors);
        }
        
        public IncludeCacheEntry CreatedEntry { get; }
        
        public IReadOnlyCollection<CodeSpecificException> ParsingErrors { get; }

        protected override string DataToString()
        {
            return $"{nameof(CreatedEntry)}: {CreatedEntry}; {nameof(ParsingErrors)}: {string.Join(", ", ParsingErrors)}";
        }
    }
}
