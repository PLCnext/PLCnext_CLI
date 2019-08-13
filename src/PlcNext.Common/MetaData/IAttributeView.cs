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
    public interface IAttributeView
    {
        IEnumerable<string> Attributes(IField field);
        IEnumerable<string> Attributes(Port port);
    }
}
