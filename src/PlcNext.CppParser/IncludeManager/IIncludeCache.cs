﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.IncludeManager
{
    internal interface IIncludeCache
    {
        void LoadCache(VirtualFile cacheFile);
        bool TryGetCacheEntry(string pathToInclude, out IncludeCacheEntry cacheEntry);
        bool TryGetCacheEntryWithTypeName(string typeFullName, IReadOnlyCollection<string> includeDirectories,
                                          out IncludeCacheEntry cacheEntry);
        IIncludeCacheTransaction StartTransaction();
        IEnumerable<IncludeCacheEntry> Entries { get; }
        Version Version { get; }
        void SaveCacheTo(Stream saveStream);
        void DeleteEntries(IEnumerable<IncludeCacheEntry> deletableEntries);
        void AddEntries(IEnumerable<IncludeCacheEntry> addedEntries);
    }

    internal interface IIncludeCacheTransaction : IDisposable
    {
        void DeleteEntry(IncludeCacheEntry cacheEntry);
        void AddEntry(IncludeCacheEntry cacheEntry);
    }
}