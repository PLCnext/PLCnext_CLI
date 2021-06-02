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

namespace PlcNext.CppParser.Messages
{
    public class IncludeCacheUpdateParsingErrors : Message
    {
        public IncludeCacheUpdateParsingErrors(Message predecessorMessage, IReadOnlyCollection<CodeSpecificException> parsingErrors): base(predecessorMessage)
        {
            ParsingErrors = parsingErrors;
        }

        public IncludeCacheUpdateParsingErrors(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CodeSpecificException> parsingErrors): base(predecessorMessages)
        {
            ParsingErrors = parsingErrors;
        }
        
        public IReadOnlyCollection<CodeSpecificException> ParsingErrors { get; }

        protected override string DataToString()
        {
            return $"{nameof(ParsingErrors)}: {string.Join(", ", ParsingErrors)}";
        }
    }
}
