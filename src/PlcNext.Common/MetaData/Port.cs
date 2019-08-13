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
    public class Port
    {
        public Port(IField field, (string, CodePosition) attributes, string datatype)
        {
            Field = field;
            Attributes = attributes;
            Datatype = datatype;
        }

        public IField Field { get; }
        public (string Attributes, CodePosition Position) Attributes { get; }
        public string Datatype { get; }
    }
}
