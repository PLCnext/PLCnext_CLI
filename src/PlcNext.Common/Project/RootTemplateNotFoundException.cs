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

namespace PlcNext.Common.Project
{
    internal class RootTemplateNotFoundException : FormattableException
    {
        public RootTemplateNotFoundException(string templateName) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.RootTemplateNotFound, templateName))
        {
            
        }
    }
}
