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

namespace PlcNext.Common.Tools.Settings
{
    internal class BoolSettingsValueFormatException : FormattableException
    {
        public BoolSettingsValueFormatException(string key, string value) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.BoolSettingsValueFormat, key, value))
        {
            
        }
    }
}
