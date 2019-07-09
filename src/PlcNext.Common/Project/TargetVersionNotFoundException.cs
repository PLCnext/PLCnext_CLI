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
using PlcNext.Common.Tools;

namespace PlcNext.Common.Project
{
    public class TargetVersionNotFoundException : FormattableException
    {
        public TargetVersionNotFoundException(string targetName, string version, string[] possibleVersions)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TargetVersionNotFound, targetName, version,
                                 possibleVersions.Aggregate(string.Empty, (c, v) => c + " " + v)))
        {

        }
    }
}
