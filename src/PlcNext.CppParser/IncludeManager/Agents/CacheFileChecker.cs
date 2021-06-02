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
    [Produces(typeof(CacheFileNotFound))]
    [Intercepts(typeof(CacheFileFound))]
    internal class CacheFileChecker : InterceptorAgent
    {
        private readonly IFileSystem fileSystem;
        public CacheFileChecker(IMessageBoard messageBoard, IFileSystem fileSystem) : base(messageBoard)
        {
            this.fileSystem = fileSystem;
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            CacheFileFound fileFound = messageData.Get<CacheFileFound>();
            if (fileSystem.FileExists(fileFound.Path))
            {
                return InterceptionAction.Continue;
            }
            OnMessage(new CacheFileNotFound(messageData, fileFound.Path));
            return InterceptionAction.DoNotPublish;
        }
    }
}
