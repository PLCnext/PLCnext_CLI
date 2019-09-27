#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Tools.SDK
{
    public class SdkInformation
    {
        public SdkInformation(IEnumerable<Target> targets, IEnumerable<string> includePaths, VirtualDirectory root, VirtualFile makeFile, CompilerInformation compilerInformation)
        {
            Targets = targets;
            Root = root;
            MakeFile = makeFile;
            CompilerInformation = compilerInformation;
            IncludePaths = includePaths;
        }

        public CompilerInformation CompilerInformation { get; }

        public IEnumerable<Target> Targets { get; }

        public IEnumerable<string> IncludePaths { get; }

        public VirtualDirectory Root { get; }

        public VirtualFile MakeFile { get; }
    }
}
