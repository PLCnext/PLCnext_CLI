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

namespace PlcNext.Common.Tools.SDK
{
    internal class NoSdkForTargetException : FormattableException
    {
        public NoSdkForTargetException(Target target) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.NoSdkForTarget, target.GetLongFullName()))
        {
            
        }
    }
}
