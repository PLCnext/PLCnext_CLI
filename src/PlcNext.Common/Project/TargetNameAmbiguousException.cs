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
using System.Linq;
using System.Text;

namespace PlcNext.Common.Project
{
    public class TargetNameAmbiguousException :  FormattableException
    {
        public TargetNameAmbiguousException(string target, IEnumerable<string> possibleTargets) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.TargetNameAmbiguous, target, possibleTargets.Aggregate(string.Empty, (c, v) => c + Environment.NewLine + "\t- " + v)))
        {

        }
    }
}
