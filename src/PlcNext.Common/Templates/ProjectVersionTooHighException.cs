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
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Templates
{
    internal class ProjectVersionTooHighException : FormattableException
    {
        public ProjectVersionTooHighException(string projectVersion, string  foundProjectVersion) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.ProjectVersionMismatch, foundProjectVersion, projectVersion))
        {
            
        }
    }
}
