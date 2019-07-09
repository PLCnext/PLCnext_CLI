#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Globalization;

namespace PlcNext.Common.Tools.Web
{
    internal class DownloadFileNotReachableException : FormattableException
    {
        public DownloadFileNotReachableException(Uri webUrl) : this(webUrl, null)
        {

        }
        public DownloadFileNotReachableException(Uri webUrl, Exception innerException) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.DownloadFileUnreachable, webUrl), innerException)
        {

        }
    }
}
