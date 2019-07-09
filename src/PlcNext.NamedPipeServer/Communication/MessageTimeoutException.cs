#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using PlcNext.Common.Tools;

namespace PlcNext.NamedPipeServer.Communication
{
    public class MessageTimeoutException : Exception
    {
        public MessageTimeoutException(Guid messageGuid, int timeout, Exception innerException) : base(string.Format(ExceptionTexts.MessageTimeout,timeout,messageGuid.ToByteString()), innerException)
        {
            
        }
    }
}
