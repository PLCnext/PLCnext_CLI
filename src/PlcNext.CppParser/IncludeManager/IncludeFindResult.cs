#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.IncludeManager
{
    internal sealed class IncludeFindResult
    {
        public VirtualFile IncludeFile { get; }
        public VirtualDirectory IncludeBaseDirectory { get; }
        public IncludeCacheEntry IncludeCacheEntry { get; }
        public bool IsInCache => IncludeCacheEntry != null;

        public IncludeFindResult(VirtualFile includeFile, VirtualDirectory includeBaseDirectory)
        {
            IncludeFile = includeFile;
            IncludeBaseDirectory = includeBaseDirectory;
        }

        public IncludeFindResult(IncludeCacheEntry includeCacheEntry)
        {
            IncludeCacheEntry = includeCacheEntry;
        }

        public override string ToString()
        {
            return $"{nameof(IncludeFile)}: {IncludeFile}, {nameof(IncludeBaseDirectory)}: {IncludeBaseDirectory}, {nameof(IncludeCacheEntry)}: {IncludeCacheEntry}, {nameof(IsInCache)}: {IsInCache}";
        }
    }
}