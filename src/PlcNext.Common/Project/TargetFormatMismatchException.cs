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

namespace PlcNext.Common.Project
{
    internal class TargetFormatMismatchException : FormattableException
    {
        public TargetFormatMismatchException(string target, bool withFile)
            : base(string.Format(CultureInfo.InvariantCulture, withFile
                                                                   ? ExceptionTexts.TargetFormatMismatchWithFile
                                                                   : ExceptionTexts.TargetFormatMismatch,
                                 target))
        {

        }
    }
}
