#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Agents.Net;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.Agents
{
    [Produces(typeof(CppFileOpened))]
    [Intercepts(typeof(CppFileFound))]
    internal  class FileOpener : InterceptorAgent
    {
        public FileOpener(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            CppFileFound fileFound = messageData.Get<CppFileFound>();
            CppFileOpened.Decorate(fileFound, fileFound.File.OpenRead());
            return InterceptionAction.Continue;
        }
    }
}
