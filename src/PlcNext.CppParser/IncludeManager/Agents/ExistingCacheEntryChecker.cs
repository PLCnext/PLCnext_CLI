#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(IncludeFileFound))]
    [Produces(typeof(IncludeFileProcessed))]
    [Produces(typeof(IncludeFileParsing))]
    internal class ExistingCacheEntryChecker : Agent
    {
        private readonly IFileSystem fileSystem;
        public ExistingCacheEntryChecker(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
        }

        protected override void ExecuteCore(Message messageData)
        {
            IncludeFileFound fileFound = messageData.Get<IncludeFileFound>();
            if (fileFound.FindResult.IsInCache)
            {
                VirtualFile cacheSourceFile = fileSystem.GetFile(fileFound.FindResult.IncludeCacheEntry.File);
                VirtualDirectory cacheBaseDirectory = fileSystem.GetDirectory(fileFound.FindResult.IncludeCacheEntry.BaseDirectory);
                string relativePath = cacheSourceFile.GetRelativePath(cacheBaseDirectory);
                IncludeDefinition[] childDefinitions = fileFound.FindResult.IncludeCacheEntry.Includes
                                                                .Select(i => new IncludeDefinition(i, cacheSourceFile,
                                                                            cacheBaseDirectory))
                                                                .ToArray();
                OnMessage(new IncludeFileProcessed(messageData, fileFound.FindResult.IncludeCacheEntry, childDefinitions, relativePath));
            }
            else
            {
                OnMessage(new IncludeFileParsing(messageData,
                                                 fileFound.FindResult.IncludeFile,
                                                 fileFound.FindResult.IncludeBaseDirectory));
            }
        }
    }
}
