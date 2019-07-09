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

namespace PlcNext.Common.Project
{
    public class TargetAlreadySupportedException : FormattableException
    {
        public TargetAlreadySupportedException(string target) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TargetAlreadySupported, target))
        {

        }
    }
}
