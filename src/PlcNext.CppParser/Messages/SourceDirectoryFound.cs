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
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.CppParser.Messages
{
    public class SourceDirectoryFound : Message
    {
        public SourceDirectoryFound(Message predecessorMessage, VirtualDirectory directory): base(predecessorMessage)
        {
            Directory = directory;
        }

        public SourceDirectoryFound(IEnumerable<Message> predecessorMessages, VirtualDirectory directory): base(predecessorMessages)
        {
            Directory = directory;
        }
        
        public VirtualDirectory Directory { get; }

        protected override string DataToString()
        {
            return $"{nameof(Directory)}: {Directory}";
        }
    }
}
