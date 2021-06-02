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
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(SourceDirectoryFound))]
    [Produces(typeof(CppFileFound))]
    [Produces(typeof(EmptyDirectoryFound))]
    internal class CppFileFinder : Agent
    {
        public CppFileFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            SourceDirectoryFound directoryFound = messageData.Get<SourceDirectoryFound>();
            CppFileFound[] messages = directoryFound.Directory.Files("*.hpp", true)
                                                    .Select(f => new CppFileFound(messageData, f,
                                                                directoryFound.Directory))
                                                    .ToArray();
            if (messages.Any())
            {
                OnMessages(messages);
            }
            else
            {
                OnMessage(EmptyDirectoryFound.Create(messageData));
            }
        }
    }
}
