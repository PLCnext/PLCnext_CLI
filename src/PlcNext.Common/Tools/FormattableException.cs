#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PlcNext.Common.Tools
{
    public class FormattableException : Exception
    {
        public FormattableException()
        {
        }

        protected FormattableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FormattableException(string message) : base(message)
        {
        }

        public FormattableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
