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
using PlcNext.Common.CodeModel;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.CppParser.CppRipper;

namespace PlcNext.CppParser.Messages
{
    internal class FileResult
    {
        private readonly List<ParserMessage> errors = new List<ParserMessage>();
        public Dictionary<IType, CodePosition> Types { get; } = new Dictionary<IType, CodePosition>();
        public IReadOnlyCollection<string> Includes { get; } = Array.Empty<string>();
        public Dictionary<string, string> DefineStatements { get; } = new Dictionary<string, string>();
        public VirtualFile File { get; }
        public VirtualDirectory RootDirectory { get; }
        public IReadOnlyCollection<ParserMessage> Errors => errors;
        public bool ParsedSuccessfully { get; }

        public FileResult(Dictionary<IType, CodePosition> types, IReadOnlyCollection<string> includes, Dictionary<string, string> defineStatements, VirtualFile file, VirtualDirectory rootDirectory)
        {
            Types = types;
            Includes = includes;
            File = file;
            RootDirectory = rootDirectory;
            DefineStatements = defineStatements;
            ParsedSuccessfully = true;
        }

        public FileResult(ParserMessage error, VirtualFile file, VirtualDirectory rootDirectory)
        {
            errors.Add(error);
            File = file;
            RootDirectory = rootDirectory;
            ParsedSuccessfully = false;
        }
        
        public void AddErrorMessages(IEnumerable<ParserMessage> parserMessages)
        {
            errors.AddRange(parserMessages);
        }

        public override string ToString()
        {
            return $"{nameof(Types)}: {Types.Count}, {nameof(Includes)}: {Includes.Count}, {nameof(File)}: {File}, {nameof(RootDirectory)}: {RootDirectory}, {nameof(Errors)}: {Errors.Count}";
        }
    }
}