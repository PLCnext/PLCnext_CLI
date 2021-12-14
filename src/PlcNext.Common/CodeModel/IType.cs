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

namespace PlcNext.Common.CodeModel
{
    public interface IType : ICodeEntity
    {
        IEnumerable<IDataType> BaseTypes { get; }
        string Namespace { get; }
        string FullName { get; }
        IEnumerable<IField> Fields { get; }
        IEnumerable<string> AccessibleNamespaces { get; }
    }
}
