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
using System.Security;
using System.Text;

#pragma warning disable CA1711

namespace PlcNext.Common.CodeModel
{
    public interface IAttribute
    {
        CodePosition Position { get; }
        string Name { get; }
        IEnumerable<string> Values { get; }
        IDictionary<string, string> NamedValues { get; }
    }
}
