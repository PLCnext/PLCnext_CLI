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

namespace PlcNext.Common.Templates
{
    public class MultipleRootNamespacesForTargetException : FormattableException
    {
        public MultipleRootNamespacesForTargetException(IEnumerable<string> rootNamespaces) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.MultipleRootNamespacesForTarget, Environment.NewLine+string.Join(", ",rootNamespaces))) 
        {
            
        }
    }
}
