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

namespace PlcNext.Common.CodeModel
{
    internal class UnknownIecDataTypeException : FormattableException
    {
        public UnknownIecDataTypeException(string dataType) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.UnknownIecDataType, dataType))
        {

        }
    }
}
