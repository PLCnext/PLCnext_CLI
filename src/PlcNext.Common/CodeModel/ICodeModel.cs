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
    public interface ICodeModel
    {
        IDictionary<IStructure, VirtualFile> Structures { get; }
        IDictionary<IClass, VirtualFile> Classes { get; }
        IDictionary<IEnum, VirtualFile> Enums { get; }
        IDictionary<IType, VirtualFile> Types { get; }
        IStructure Structure(string structureName);
        IClass Class(string className);
        IEnum Enum(string className);
        IType Type(string typeName);
        VirtualDirectory GetBaseDirectory(IType type);
    }
}
