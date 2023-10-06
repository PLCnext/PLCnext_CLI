#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.CppParser.IncludeManager
{
    internal sealed class IncludeCache
    {
        public Version Version { get; set; }
        
        public IncludeCacheEntry[] CacheEntries { get; set; }
    }
}