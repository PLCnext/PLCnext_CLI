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

namespace PlcNext.Common.Tools
{
    internal class CycleException : FormattableException
    {
        public CycleException(string actionDescription, string cycle) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.Cycle, actionDescription, $"{Environment.NewLine}{cycle}"))
        {
            
        }
    }
}
