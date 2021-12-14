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
using System.Linq;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    internal interface IFileParser
    {
        ParserResult Parse(VirtualFile file);
    }

    internal class ParserResult
    {
        public ParserResult(IEnumerable<CodeSpecificException> exceptions, IDictionary<IType, CodePosition> types, string[] includes, Dictionary<string, string> defineStatements,
                            IDictionary<IConstant, CodePosition> constants, bool success)
        {
            Exceptions = exceptions;
            Types = types;
            Includes = includes;
            Success = success;
            DefineStatements = defineStatements;
            Constants = constants;
        }

        public ParserResult(IEnumerable<CodeSpecificException> exceptions, IDictionary<IType, CodePosition> types,
                            string[] includes, Dictionary<string, string> defineStatements,
                            IDictionary<IConstant, CodePosition> constants)
            : this(exceptions, types, includes, defineStatements, constants, true)
        {
        }

        public ParserResult(IEnumerable<CodeSpecificException> exceptions)
            : this(exceptions, new Dictionary<IType, CodePosition>(), Array.Empty<string>(), new Dictionary<string, string>(), new Dictionary<IConstant, CodePosition>(), false)
        {
        }

        public IEnumerable<CodeSpecificException> Exceptions { get; }
        public IDictionary<IType, CodePosition> Types { get; }
        public string[] Includes { get; }
        public bool Success { get; }
        public Dictionary<string, string> DefineStatements { get; }
        public IDictionary<IConstant, CodePosition> Constants { get; }
    }
}
