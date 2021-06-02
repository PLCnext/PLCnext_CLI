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

namespace PlcNext.CppParser.IncludeManager.Messages
{
    public class IncludeFileParsing : Message
    {
        public IncludeFileParsing(Message predecessorMessage, VirtualFile file, VirtualDirectory rootDirectory): base(predecessorMessage)
        {
            File = file;
            RootDirectory = rootDirectory;
        }

        public IncludeFileParsing(IEnumerable<Message> predecessorMessages, VirtualFile file, VirtualDirectory rootDirectory): base(predecessorMessages)
        {
            File = file;
            RootDirectory = rootDirectory;
        }
        
        public VirtualFile File { get; }
        public VirtualDirectory RootDirectory { get; }

        protected override string DataToString()
        {
            return $"{nameof(File)}: {File}; {nameof(RootDirectory)}:{RootDirectory}";
        }
    }
}
