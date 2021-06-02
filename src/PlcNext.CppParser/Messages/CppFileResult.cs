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
using PlcNext.CppParser.CppRipper;

namespace PlcNext.CppParser.Messages
{
    internal class CppFileResult : Message
    {
        public CppFileResult(Message predecessorMessage, FileResult fileResult): base(predecessorMessage)
        {
            FileResult = fileResult;
        }

        public CppFileResult(IEnumerable<Message> predecessorMessages, FileResult fileResult): base(predecessorMessages)
        {
            FileResult = fileResult;
        }
        
        public FileResult FileResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(FileResult)}: {FileResult}";
        }

        public void AddErrorMessages(IEnumerable<ParserMessage> parserMessages)
        {
            FileResult.AddErrorMessages(parserMessages);
        }
    }
}
