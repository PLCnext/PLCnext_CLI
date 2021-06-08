#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Threading;
using Agents.Net;

namespace PlcNext.CppParser.Messages
{
    public class ExceptionCancellationTokenCreated : Message
    {
        public ExceptionCancellationTokenCreated(Message predecessorMessage,
                                                 CancellationTokenSource cancellationTokenSource,
                                                 CancellationToken token): base(predecessorMessage)
        {
            Token = token;
            CancellationTokenSource = cancellationTokenSource;
        }

        public ExceptionCancellationTokenCreated(IEnumerable<Message> predecessorMessages,
                                                 CancellationTokenSource cancellationTokenSource,
                                                 CancellationToken token): base(predecessorMessages)
        {
            Token = token;
            CancellationTokenSource = cancellationTokenSource;
        }
        
        public CancellationToken Token { get; }
        public CancellationTokenSource CancellationTokenSource { get; }

        protected override string DataToString()
        {
            return string.Empty;
        }
    }
}
