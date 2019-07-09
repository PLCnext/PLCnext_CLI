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

namespace PlcNext.Common.Tools.Security
{
    internal class SignatureValidationException : FormattableException
    {
        public string ValidationFileName { get; set; }

        public override string Message => string.Format(CultureInfo.InvariantCulture, ExceptionTexts.SignatureValidationFailure,
                                                        ValidationFileName);
    }
}
