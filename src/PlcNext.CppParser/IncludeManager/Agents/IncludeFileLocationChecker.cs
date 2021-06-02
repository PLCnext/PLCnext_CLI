#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Concurrent;
using Agents.Net;
using ConcurrentCollections;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(InitialIncludeDefinitionsDefined))]
    [Produces(typeof(IncludeFileProcessUnnecessary))]
    [Intercepts(typeof(IncludeFileFound))]
    internal class IncludeFileLocationChecker : InterceptorAgent
    {
        private readonly ConcurrentDictionary<MessageDomain, ConcurrentHashSet<string>> parsedFiles =
            new ConcurrentDictionary<MessageDomain, ConcurrentHashSet<string>>();
        public IncludeFileLocationChecker(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            InitialIncludeDefinitionsDefined includeDefinitionsDefined =
                messageData.Get<InitialIncludeDefinitionsDefined>();
            
            ConcurrentHashSet<string> parsedFilesOfModel = new ConcurrentHashSet<string>(includeDefinitionsDefined.ParsedFiles);
            parsedFiles.TryAdd(includeDefinitionsDefined.MessageDomain, parsedFilesOfModel);
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            IncludeFileFound fileFound = messageData.Get<IncludeFileFound>();
            string relativePath = fileFound.FindResult.IsInCache
                                      ?fileFound.FindResult.IncludeCacheEntry.File.Substring(fileFound.FindResult.IncludeCacheEntry.BaseDirectory.Length+1)
                                      :fileFound.FindResult.IncludeFile.GetRelativePath(fileFound.FindResult.IncludeBaseDirectory); 
            
            MessageDomain domain = fileFound.MessageDomain;
            while (domain != null)
            {
                if (parsedFiles.TryGetValue(domain, out ConcurrentHashSet<string> files))
                {
                    if (!files.Add(relativePath))
                    {
                        OnMessage(new IncludeFileProcessUnnecessary(messageData));
                        return InterceptionAction.DoNotPublish;
                    }

                    break;
                }
                domain = domain.Parent;
            }

            return InterceptionAction.Continue;
        }
    }
}
