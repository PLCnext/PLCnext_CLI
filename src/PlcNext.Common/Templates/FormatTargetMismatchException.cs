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
using PlcNext.Common.Templates.Format;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    public class FormatTargetMismatchException : FormattableException
    {
        public FormatTargetMismatchException(formatTemplate template, string entityType) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.FormatTargetMismatch, template?.name, entityType, template.target))
        {
            
        }

        public FormatTargetMismatchException(string templateName, string target, string entityType) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.FormatTargetMismatch, templateName, entityType, target))
        {
            
        }
    }
}
