#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using Agents.Net;
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.IncludeManager;

namespace PlcNext.CppParser.Messages
{
    internal class CodeModelCreated : Message
    {
        public CodeModelCreated(Message predecessorMessage, CppCodeModel codeModel,
                                IReadOnlyCollection<IncludeDefinition> includes,
                                IReadOnlyCollection<CodeSpecificException> exceptions,
                                IReadOnlyCollection<string> parsedFiles): base(predecessorMessage)
        {
            CodeModel = codeModel;
            Includes = includes;
            Exceptions = exceptions;
            ParsedFiles = parsedFiles;
        }

        public CodeModelCreated(IEnumerable<Message> predecessorMessages, CppCodeModel codeModel, IReadOnlyCollection<IncludeDefinition> includes, IReadOnlyCollection<CodeSpecificException> exceptions, IReadOnlyCollection<string> parsedFiles): base(predecessorMessages)
        {
            CodeModel = codeModel;
            Includes = includes;
            Exceptions = exceptions;
            ParsedFiles = parsedFiles;
        }
        
        public CppCodeModel CodeModel { get; }
        public IReadOnlyCollection<IncludeDefinition> Includes { get; }
        public IReadOnlyCollection<CodeSpecificException> Exceptions { get; }
        public IReadOnlyCollection<string> ParsedFiles { get; }

        protected override string DataToString()
        {
            return $"{nameof(CodeModel)}: {CodeModel}";
        }
    }
}
