#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(CacheFileFound))]
    [Produces(typeof(CacheFileOpened))]
    internal class CacheFileOpener : Agent
    {
        private IFileSystem fileSystem;
        public CacheFileOpener(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new CacheFileOpened(messageData, fileSystem.GetFile(messageData.Get<CacheFileFound>().Path).OpenRead()));
        }
    }
}
