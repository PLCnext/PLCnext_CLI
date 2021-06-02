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
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;
using Serilog;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(RegisteringIncludesToCodeModel))]
    [Consumes(typeof(CacheParsed))]
    [Consumes(typeof(IncludeDirectoriesFound))]
    [Consumes(typeof(IncludeCacheFileParsed))]
    [Produces(typeof(IncludesRegisteredToCodeModel))]
    [Produces(typeof(IncludeCacheFileParsing))]
    //This is the decorator for parsing existing cache files when looking for a specific type
    internal class CodeModelIncludesRegister : Agent
    {
        private readonly MessageCollector<RegisteringIncludesToCodeModel, IncludeDirectoriesFound, CacheParsed>
            collector;

        private readonly MessageCollector<IncludeCacheFileParsing, IncludeCacheFileParsed> findCollector =
            new MessageCollector<IncludeCacheFileParsing, IncludeCacheFileParsed>();

        private readonly IFileSystem fileSystem;
        public CodeModelIncludesRegister(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
            collector =
                new MessageCollector<RegisteringIncludesToCodeModel, IncludeDirectoriesFound, CacheParsed>(
                    OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<RegisteringIncludesToCodeModel, IncludeDirectoriesFound, CacheParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);
            set.MarkAsConsumed(set.Message3);

            CppCodeModel codeModel = set.Message1.CodeModel;
            codeModel.IncludeDirectories = set.Message2.IncludeDirectories;
            codeModel.RegisterIncludeTypeFinder(fullName => FindIncludeType(fullName, set.Message3.Cache,
                                                                            set.Message2.IncludeDirectories, codeModel,
                                                                            set));
            OnMessage(new IncludesRegisteredToCodeModel(set));
        }
        
        private IType FindIncludeType(string fullName, IIncludeCache includeCache, IEnumerable<IncludePath> includeDirectories, CppCodeModel codeModel, IEnumerable<Message> predecessors)
        {
            if (!includeCache.TryGetCacheEntryWithTypeName(fullName, includeDirectories.Select(x => x.Directory)
                                                              .Select(d => d?.FullName ?? string.Empty)
                                                              .ToArray(),
                                                           out IncludeCacheEntry cacheEntry))
            {
                return null;
            }

            Log.Information($"Parse type {fullName} from {cacheEntry.File}");
            VirtualFile cacheFile = fileSystem.GetFile(cacheEntry.File);
            VirtualDirectory cacheBaseDirectory = fileSystem.GetDirectory(cacheEntry.BaseDirectory);

            IncludeCacheFileParsing fileParsing = IncludeCacheFileParsing.Decorate(new CppFileFound(predecessors, cacheFile, cacheBaseDirectory));
            MessageDomain.CreateNewDomainsFor(fileParsing);
            OnMessage(fileParsing);
            
            IncludeCacheFileParsed fileParsed = null;
            findCollector.PushAndExecute(fileParsing, set =>
            {
                set.MarkAsConsumed(set.Message1);
                set.MarkAsConsumed(set.Message2);
                fileParsed = set.Message2;
            });
            
            MessageDomain.TerminateDomainsOf(fileParsed);

            IEnumerable<CodeSpecificException> exceptions = fileParsed.FileResult.Errors.Select(e => e.ToException(cacheFile));
            foreach (CodeSpecificException codeSpecificException in exceptions)
            {
                Log.Warning(codeSpecificException.Message);
            }
                
            IType result = null;
            foreach (IType type in fileParsed.FileResult.Types.Keys)
            {
                codeModel?.AddType(type, cacheFile, cacheBaseDirectory);
                if (type.FullName == fullName)
                {
                    result = type;
                }
            }
                
            Log.Information(result != null
                                ? $"Successfully retrieved type {fullName}."
                                : $"Could not find the type {fullName} in {cacheEntry.File}. This should not happen.");
            return result;
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (!collector.TryPush(messageData))
            {
                findCollector.Push(messageData);
            }
        }
    }
}
