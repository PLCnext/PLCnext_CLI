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
    public class CppFileFound : Message
    {
        public CppFileFound(Message predecessorMessage, VirtualFile file, VirtualDirectory directory): base(predecessorMessage)
        {
            File = file;
            Directory = directory;
        }

        public CppFileFound(IEnumerable<Message> predecessorMessages, VirtualFile file, VirtualDirectory directory): base(predecessorMessages)
        {
            File = file;
            Directory = directory;
        }
        
        public VirtualFile File { get; }
        public VirtualDirectory Directory { get; }

        protected override string DataToString()
        {
            return $"{nameof(File)}: {File}";
        }
    }
}
