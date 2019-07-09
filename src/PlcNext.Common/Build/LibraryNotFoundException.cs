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

namespace PlcNext.Common.Build
{
    internal class LibraryNotFoundException : FormattableException
    {
        public LibraryNotFoundException(string libraryLocation) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.LibraryNotFound, libraryLocation))
        {
            
        }
    }
}
