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

namespace PlcNext.Common.Build
{
    internal class CMakeBuildSystemNotFoundException : FormattableException
    {
        public CMakeBuildSystemNotFoundException(string path) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.CMakeBuildSystemNotFound, path))
        {

        }

        public CMakeBuildSystemNotFoundException(string target, string buildType) 
        :base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.CMakeBuildSystemNotFoundWithoutPath, target, buildType))
        {
            
        }
    }
}
