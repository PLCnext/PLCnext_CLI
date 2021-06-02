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
    [Consumes(typeof(IncludeFileParsing))]
    [Produces(typeof(IncludeCppFileFound))]
    //This is the decorator for parsing new files to add to the cache
    internal class IncludeFileParsingDecorator : Agent
    {
        public IncludeFileParsingDecorator(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            IncludeFileParsing fileParsing = messageData.Get<IncludeFileParsing>();
            IncludeCppFileFound fileFound = IncludeCppFileFound.Decorate(new CppFileFound(messageData, fileParsing.File, fileParsing.RootDirectory));
            MessageDomain.CreateNewDomainsFor(fileFound);
            OnMessage(fileFound);
        }
    }
}
