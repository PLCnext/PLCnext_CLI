#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CacheUpdated))]
    [Consumes(typeof(CacheFileFound))]
    public class CacheFileSaver : Agent
    {
        private readonly IFileSystem fileSystem;
        private readonly MessageCollector<CacheUpdated, CacheFileFound> collector;
        private readonly object writerLock = new object();
        public CacheFileSaver(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
            collector = new MessageCollector<CacheUpdated, CacheFileFound>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<CacheUpdated, CacheFileFound> set)
        {
            set.MarkAsConsumed(set.Message1);
            lock (writerLock)
            {
                using (Stream writeStream = fileSystem.GetFile(set.Message2.Path).OpenWrite())
                {
                    set.Message1.Get<CacheParsed>().Cache.SaveCacheTo(writeStream);                
                }
            }
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
