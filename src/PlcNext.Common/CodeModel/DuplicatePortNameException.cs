#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PlcNext.Common.CodeModel
{
    internal class DuplicatePortNameException : FormattableException
    {
        public DuplicatePortNameException(string name) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.DuplicatePortName, name))
        {

        }
    }
}
