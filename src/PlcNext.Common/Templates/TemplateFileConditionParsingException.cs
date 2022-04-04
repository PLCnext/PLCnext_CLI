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

namespace PlcNext.Common.Templates
{
    internal class TemplateFileConditionParsingException : FormattableException
    {
        public TemplateFileConditionParsingException(string condition) : 
            base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TemplateFileConditionParsing, condition))
        {
                
        }

        public TemplateFileConditionParsingException(string condition, Exception ex) :
            base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TemplateFileConditionParsing, condition), ex)
        {

        }
    }
}
