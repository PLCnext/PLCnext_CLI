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
using System.Globalization;
using System.Text;

namespace PlcNext.Common.Tools.IO
{
    internal class UnsupportedArchiveFormatException : FormattableException
    {
        public UnsupportedArchiveFormatException(string archive) : this(archive, null)
        {

        }

        public UnsupportedArchiveFormatException(string archive, Exception innerException) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.UnsupportedArchiveFormat, archive), innerException)
        {

        }
    }
}
