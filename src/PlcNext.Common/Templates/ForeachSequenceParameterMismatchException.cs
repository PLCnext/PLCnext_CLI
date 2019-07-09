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
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    internal class ForeachSequenceParameterMismatchException : FormattableException
    {
        public ForeachSequenceParameterMismatchException(string parameter) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.ForeachSequenceParameterMismatch, parameter))
        {
            
        }
    }
}
