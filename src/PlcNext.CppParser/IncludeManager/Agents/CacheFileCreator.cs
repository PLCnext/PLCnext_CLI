#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using System.Text;
using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CacheFileNotFound))]
    [Produces(typeof(CacheFileFound))]
    internal class CacheFileCreator : Agent
    {
        private readonly IFileSystem fileSystem;
        public CacheFileCreator(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
        }

        protected override void ExecuteCore(Message messageData)
        {
            CacheFileNotFound notFound = messageData.Get<CacheFileNotFound>();
            using (Stream fileStream = fileSystem.GetFile(notFound.Path).OpenWrite())
            using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                writer.Write("[]");
            }
            OnMessage(new CacheFileFound(messageData, notFound.Path));
        }
    }
}
