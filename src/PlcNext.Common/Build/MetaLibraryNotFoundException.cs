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
    internal class MetaLibraryNotFoundException : FormattableException
    {
        public MetaLibraryNotFoundException(string metaDirectory) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.MetaLibraryNotFound, metaDirectory))
        {
            
        }
    }
}
