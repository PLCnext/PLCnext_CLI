#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace PlcNext.Common.CodeModel
{
    public interface IConstant : ICodeEntity
    {
        string Namespace { get; }
        
        string Value { get; }
        
        string FullName { get; }
        
        IEnumerable<string> AccessibleNamespaces { get; }
    }
}