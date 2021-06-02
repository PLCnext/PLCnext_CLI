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
using PlcNext.CppParser.IncludeManager.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Consumes(typeof(CodeModelCreationParameters))]
    [Produces(typeof(SourceDirectoryFound))]
    [Produces(typeof(NoSourceDirectoriesFound))]
    [Produces(typeof(IncludeDirectoriesFound))]
    [Produces(typeof(LoadingIncludeCache))]
    internal class CodeModelCreationStarter : Agent
    {
        public CodeModelCreationStarter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CodeModelCreationParameters creationParameters = messageData.Get<CodeModelCreationParameters>();
            OnMessage(new LoadingIncludeCache(messageData));;
            OnMessage(new IncludeDirectoriesFound(messageData, creationParameters.IncludeDirectories));
            if (creationParameters.SourceDirectories.Any())
            {
                OnMessages(creationParameters.SourceDirectories.Select(d => new SourceDirectoryFound(messageData, d))
                                             .ToArray());
            }
            else
            {
                OnMessage(NoSourceDirectoriesFound.Create(messageData));
            }
        }
    }
}
