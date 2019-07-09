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
    public class TargetNameNotFoundException : FormattableException
    {
        public TargetNameNotFoundException(string targetName) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TargetNameNotFound, targetName))
        {

        }
        public TargetNameNotFoundException(string targetName, string closestTarget) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TargetNameNotFoundClosestMatch, targetName, closestTarget))
        {

        }
    }
}
