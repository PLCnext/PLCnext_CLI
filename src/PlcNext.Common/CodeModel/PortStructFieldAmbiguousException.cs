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
using PlcNext.Common.Tools;

namespace PlcNext.Common.CodeModel
{
    public class PortStructFieldAmbiguousException : FormattableException
    {
        public PortStructFieldAmbiguousException(string @type, IEnumerable<string> fields) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.PortStructureFieldAmbiguous, @type, string.Join(", ", fields)))
        {

        }
    }
}