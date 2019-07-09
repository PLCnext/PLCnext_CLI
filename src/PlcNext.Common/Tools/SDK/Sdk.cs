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
    public class Sdk
    {
        public Sdk(IEnumerable<Target> targets, IEnumerable<string> includePaths, VirtualDirectory root, VirtualFile makeFile, Compiler compiler)
        {
            Targets = targets;
            Root = root;
            MakeFile = makeFile;
            Compiler = compiler;
            IncludePaths = includePaths;
        }

        public Compiler Compiler { get; }

        public IEnumerable<Target> Targets { get; }

        public IEnumerable<string> IncludePaths { get; }

        public VirtualDirectory Root { get; }

        public VirtualFile MakeFile { get; }
    }
}
