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

namespace PlcNext.Common.Deploy
{
    internal class DeployFileNotFoundException : FormattableException
    {
        public DeployFileNotFoundException(string file) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.DeploymentFileNotFound, file))
        {
            
        }
    }
}
