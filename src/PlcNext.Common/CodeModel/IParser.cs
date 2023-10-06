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
using PlcNext.Common.Tools.SDK;

#pragma warning disable CA1021

namespace PlcNext.Common.CodeModel
{
    public interface IParser
    {
        ICodeModel Parse(ICollection<VirtualDirectory> sourceDirectories,
                         IEnumerable<IncludePath> includeDirectories,
                         out IEnumerable<CodeSpecificException> loggableExceptions);
    }
}
