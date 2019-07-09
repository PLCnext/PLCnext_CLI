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
using System.Runtime.Serialization;
using System.Text;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Project
{
    internal class FileExistsException : FormattableException
    {
        public FileExistsException(string fileName, string templateName) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.FileExists, fileName, Environment.NewLine, templateName))
        {
        }

        public FileExistsException(string destinationDirectory) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.FilesExistingInsideDirectory, destinationDirectory, Environment.NewLine))
        {
            
        }
    }
}
