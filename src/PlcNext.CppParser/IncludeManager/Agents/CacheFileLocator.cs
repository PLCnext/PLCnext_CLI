#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Consumes(typeof(LoadingIncludeCache))]
    [Produces(typeof(CacheFileFound))]
    public class CacheFileLocator : Agent
    {
        private readonly IEnvironmentService environmentService;
        private const string IncludeFilePath = "{ApplicationData}/{ApplicationName}/include-cache.json";
        public CacheFileLocator(IMessageBoard messageBoard, IEnvironmentService environmentService) : base(messageBoard)
        {
            this.environmentService = environmentService;
        }

        protected override void ExecuteCore(Message messageData)
        {
            OnMessage(new CacheFileFound(messageData, IncludeFilePath.CleanPath().ResolvePathName(environmentService.PathNames)));
        }
    }
}
