#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Agents
{
    [Produces(typeof(IncludeCacheFileParsed))]
    [Intercepts(typeof(CppFileResult))]
    //This is the finisher for parsing existing cache files when looking for a specific type
    internal class IncludeCacheFileResultCollector : InterceptorAgent
    {
        public IncludeCacheFileResultCollector(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.MessageDomain.Root.Is<IncludeCacheFileParsing>())
            {
                CppFileResult fileResult = messageData.Get<CppFileResult>();
                OnMessage(new IncludeCacheFileParsed(messageData, fileResult.FileResult));
                return InterceptionAction.DoNotPublish;
            }

            return InterceptionAction.Continue;
        }
    }
}
