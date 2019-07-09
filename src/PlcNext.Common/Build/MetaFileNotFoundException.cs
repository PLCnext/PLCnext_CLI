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
    internal class MetaFileNotFoundException : FormattableException
    {
        public MetaFileNotFoundException(string metaDirectory, string fileName) 
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.MetaFileNotFound, metaDirectory, fileName))
        {

        }
    }
}
