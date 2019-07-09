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

namespace PlcNext.Common.Tools.SDK
{
    public class Compiler
    {
        public Compiler(string compilerPath, string sysroot, string flags, IEnumerable<string> includePaths, IEnumerable<CompilerMakro> makros)
        {
            CompilerPath = compilerPath;
            Sysroot = sysroot;
            Flags = flags;
            IncludePaths = includePaths;
            Makros = makros;
        }

        public string CompilerPath { get; }
        public string Sysroot { get; }
        public string Flags { get; }
        public IEnumerable<string> IncludePaths { get; }
        public IEnumerable<CompilerMakro> Makros { get; }
    }

    public class CompilerMakro
    {
        public CompilerMakro(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }
    }
}
