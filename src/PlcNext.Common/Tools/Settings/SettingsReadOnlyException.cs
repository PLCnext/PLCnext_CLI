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
using System.Text;

namespace PlcNext.Common.Tools.Settings
{
    internal class SettingsReadOnlyException : FormattableException
    {
        public SettingsReadOnlyException() : base(ExceptionTexts.SettingNotWritable)
        {
            
        }
    }
}
