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
using PlcNext.CppParser.CppRipper.CodeModel;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    internal class InitialIncludeDefinitionsDefined : Message
    {
        public InitialIncludeDefinitionsDefined(Message predecessorMessage, IReadOnlyCollection<IncludeDefinition> includes, IReadOnlyCollection<string> parsedFiles, CppCodeModel codeModel): base(predecessorMessage)
        {
            Includes = includes;
            ParsedFiles = parsedFiles;
            CodeModel = codeModel;
        }

        public InitialIncludeDefinitionsDefined(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<IncludeDefinition> includes, IReadOnlyCollection<string> parsedFiles, CppCodeModel codeModel): base(predecessorMessages)
        {
            Includes = includes;
            ParsedFiles = parsedFiles;
            CodeModel = codeModel;
        }
        
        public IReadOnlyCollection<IncludeDefinition> Includes { get; }
        public IReadOnlyCollection<string> ParsedFiles { get; }
        public CppCodeModel CodeModel { get; }

        protected override string DataToString()
        {
            return $"{nameof(Includes)}: {string.Join(", ", Includes)}: {nameof(ParsedFiles)}: {string.Join(", ", ParsedFiles)}";
        }
    }
}
