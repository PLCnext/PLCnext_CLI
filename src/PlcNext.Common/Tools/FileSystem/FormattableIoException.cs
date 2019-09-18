#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;

namespace PlcNext.Common.Tools.FileSystem
{
    public class FormattableIoException : FormattableException
    {
        public FormattableIoException(string message) : base(message)
        {
        }

        public FormattableIoException(IOException exception) : base(exception.Message, exception)
        {

        }
        
        public FormattableIoException(UnauthorizedAccessException exception) : base(exception.Message, exception)
        {

        }
    }
}