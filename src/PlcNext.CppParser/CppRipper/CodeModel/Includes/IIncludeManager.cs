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
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.CppRipper.CodeModel.Includes
{
    internal interface IIncludeManager
    {
        IEnumerable<CodeSpecificException> InitializeCodeModel(CppCodeModel codeModel, IncludeManagerParameter parameter);
    }

    internal class IncludeManagerParameter
    {
        public IncludeManagerParameter(IEnumerable<VirtualDirectory> includeDirectories, IEnumerable<string> knownIncludes, IEnumerable<IncludeDefinition> includeDefinitions)
        {
            IncludeDirectories = includeDirectories;
            KnownIncludes = knownIncludes;
            IncludeDefinitions = includeDefinitions;
        }

        public IEnumerable<VirtualDirectory> IncludeDirectories { get; }
        public IEnumerable<string> KnownIncludes { get; }
        public IEnumerable<IncludeDefinition> IncludeDefinitions { get; }
    }

    internal class IncludeDefinition
    {
        public IncludeDefinition(string include, VirtualFile definitionSourceFile, VirtualDirectory definitionSourceBaseDirectory)
        {
            Include = include;
            DefinitionSourceFile = definitionSourceFile;
            DefinitionSourceBaseDirectory = definitionSourceBaseDirectory;
        }

        public string Include { get; }
        public VirtualFile DefinitionSourceFile { get; }
        public VirtualDirectory DefinitionSourceBaseDirectory { get; }
    }
}
