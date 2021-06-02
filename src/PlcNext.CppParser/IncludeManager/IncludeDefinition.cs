#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.IncludeManager
{
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

        public override string ToString()
        {
            return $"{nameof(Include)}: {Include}, {nameof(DefinitionSourceFile)}: {DefinitionSourceFile}, {nameof(DefinitionSourceBaseDirectory)}: {DefinitionSourceBaseDirectory}";
        }
    }
}