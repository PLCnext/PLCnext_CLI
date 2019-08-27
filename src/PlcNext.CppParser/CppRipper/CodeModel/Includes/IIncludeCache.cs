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
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.CppRipper.CodeModel.Includes
{
    internal interface IIncludeCache
    {
        void LoadCache(VirtualFile cacheFile);
        bool TryGetCacheEntry(string pathToInclude, out IncludeCacheEntry cacheEntry);
        bool TryGetCacheEntryWithTypeName(string typeFullName, out IncludeCacheEntry cacheEntry);
        IIncludeCacheTransaction StartTransaction();
        IEnumerable<IncludeCacheEntry> Entries { get; }
    }

    internal interface IIncludeCacheTransaction : IDisposable
    {
        void DeleteEntry(IncludeCacheEntry cacheEntry);
        void AddEntry(IncludeCacheEntry cacheEntry);
    }
}