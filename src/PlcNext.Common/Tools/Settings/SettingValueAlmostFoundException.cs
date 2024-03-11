﻿#region Copyright
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
    internal class SettingValueAlmostFoundException : FormattableException
    {
        public SettingValueAlmostFoundException(string value, string closestValue) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.SettingValueAlmostFound, value, closestValue))
        {

        }
    }
}
