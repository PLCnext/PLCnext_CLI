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

namespace PlcNext.CppParser.IncludeManager.Messages
{
    public class CacheFileFound : Message
    {
        public CacheFileFound(Message predecessorMessage, string path): base(predecessorMessage)
        {
            Path = path;
        }

        public CacheFileFound(IEnumerable<Message> predecessorMessages, string path): base(predecessorMessages)
        {
            Path = path;
        }
        
        public string Path { get; }

        protected override string DataToString()
        {
            return $"{nameof(Path)}: {Path}";
        }
    }
}
