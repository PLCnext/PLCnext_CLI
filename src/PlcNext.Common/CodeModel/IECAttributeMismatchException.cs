#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using System.Globalization;

namespace PlcNext.Common.CodeModel
{
    internal class IECAttributeMismatchException : FormattableException
    {
        public IECAttributeMismatchException(string rawValue, string convertedValue)
            : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.IECDatatypeMismatch, rawValue, convertedValue))
        {

        }
    }
}
