#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.NamedPipeServer.Communication
{
    public class ClientDisconnectedException : Exception
    {
        public ClientDisconnectedException() : base(ExceptionTexts.ClientDisconnected)
        {
            
        }
        public ClientDisconnectedException(Exception innerException) : base(ExceptionTexts.ClientDisconnected, innerException)
        {
            
        }
    }
}
