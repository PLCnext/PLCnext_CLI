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

namespace PlcNext.Common.Installation
{
    internal class InvalidUriFormatException : FormattableException
    {
        public InvalidUriFormatException(string uri, bool isRelative) : this(uri, isRelative, null)
        {

        }
        public InvalidUriFormatException(string uri, bool isRelative, Exception innerException)
            : base(string.Format(CultureInfo.InvariantCulture,
                                 isRelative
                                     ? ExceptionTexts.InvalidRelativUriFormat
                                     : ExceptionTexts.InvalidUriFormat,
                                 uri),
                innerException)
        {

        }

        public static Uri TryCreateUri(string absolutUri)
        {
            try
            {
                return new Uri(PrepareAbsolutUriString(absolutUri));
            }
            catch (UriFormatException e)
            {
                throw new InvalidUriFormatException(absolutUri, false, e);
            }
        }

        public static Uri TryCreateUri(string relativeUri, Uri baseUri)
        {
            try
            {
                return new Uri(baseUri, PrepareRelativeUriString(relativeUri));
            }
            catch (UriFormatException e)
            {
                throw new InvalidUriFormatException(relativeUri, true, e);
            }
        }

        private static string PrepareAbsolutUriString(string baseUriString)
        {
            //See https://stackoverflow.com/questions/22543723/create-new-uri-from-base-uri-and-relative-path-slash-makes-a-difference for reason
            if (!baseUriString.EndsWith("/"))
            {
                baseUriString += "/";
            }

            return baseUriString;
        }

        private static string PrepareRelativeUriString(string relativeUriString)
        {
            //See https://stackoverflow.com/questions/22543723/create-new-uri-from-base-uri-and-relative-path-slash-makes-a-difference for reason
            if (relativeUriString.StartsWith("/"))
            {
                relativeUriString = relativeUriString.Substring(1);
            }

            return relativeUriString;
        }
    }
}
