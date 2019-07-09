#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.CodeModel
{
    public interface IParser
    {
        ICodeModel Parse(IEnumerable<VirtualDirectory> sourceDirectories);
    }
}
