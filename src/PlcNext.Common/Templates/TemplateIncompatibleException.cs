#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Globalization;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    public class TemplateIncompatibleException : FormattableException
    {
        public TemplateIncompatibleException(string template, string rootTemplate) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TemplateIncompatible, template, rootTemplate))
        {
            
        }
    }
}