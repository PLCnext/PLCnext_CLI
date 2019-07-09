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

namespace PlcNext.Common.Build
{
    internal class LibraryIdMalformattedException : FormattableException
    {
        public LibraryIdMalformattedException(string id) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.LibraryIdMalformatted, id))
        {
            
        }
    }
}
