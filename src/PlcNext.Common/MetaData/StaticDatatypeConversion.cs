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
using System.Text;
using PlcNext.Common.CodeModel;

namespace PlcNext.Common.MetaData
{
    internal class StaticDatatypeConversion : IDatatypeConversion
    {
        public string Convert(IDataType datatype)
        {
            string name = datatype.Name;
            int index = name.LastIndexOf("::", StringComparison.Ordinal);
            if (index != -1 && name.Length > index+2)
            {
                name = name.Substring(index + 2);
            }
            if (name.Equals("bit", StringComparison.OrdinalIgnoreCase))
            {
                return "bit";
            }
            if (name.Equals("bool", StringComparison.OrdinalIgnoreCase))
            {
                return "boolean";
            }
            else if (name.Equals("BasicString", StringComparison.OrdinalIgnoreCase))
            {
                return "String";
            }
            return name;
        }
    }
}
