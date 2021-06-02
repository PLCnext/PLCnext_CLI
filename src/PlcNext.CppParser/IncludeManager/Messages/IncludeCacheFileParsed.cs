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
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    //This is the finisher for parsing existing cache files when looking for a specific type
    internal class IncludeCacheFileParsed : Message
    {
        public IncludeCacheFileParsed(Message predecessorMessage, FileResult fileResult): base(predecessorMessage)
        {
            FileResult = fileResult;
        }

        public IncludeCacheFileParsed(IEnumerable<Message> predecessorMessages, FileResult fileResult): base(predecessorMessages)
        {
            FileResult = fileResult;
        }
        
        public FileResult FileResult { get; }

        protected override string DataToString()
        {
            return $"{nameof(FileResult)}: {FileResult}";
        }
    }
}
