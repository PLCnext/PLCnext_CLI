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

namespace PlcNext.Common.CodeModel
{
    internal class UnkownEnumDataTypeException : FormattableException
    {
        public UnkownEnumDataTypeException(string dataType, IEnum @enum) : base(string.Format(CultureInfo.InvariantCulture, ExceptionTexts.UnkownEnumDataType, dataType, @enum.FullName))
        {
            
        }
    }
}
