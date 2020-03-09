#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Globalization;
using PlcNext.Common.Tools;

namespace PlcNext.NamedPipeServer.Communication
{
    public class MessageTimeoutException : Exception
    {
        public MessageTimeoutException(int messageLength, int timeout, Exception innerException) : base(string.Format(CultureInfo.InvariantCulture,ExceptionTexts.MessageTimeout,timeout,messageLength), innerException)
        {
            
        }
    }
}
