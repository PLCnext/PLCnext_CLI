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
using PlcNext.Common.Tools.SDK;

namespace PlcNext.CppParser.Messages
{
    public class CodeModelCreationParameters : Message
    {
        public CodeModelCreationParameters(Message predecessorMessage, IEnumerable<VirtualDirectory> sourceDirectories, IEnumerable<IncludePath> includeDirectories): base(predecessorMessage)
        {
            SourceDirectories = sourceDirectories;
            IncludeDirectories = includeDirectories;
        }

        public CodeModelCreationParameters(IEnumerable<Message> predecessorMessages, IEnumerable<VirtualDirectory> sourceDirectories, IEnumerable<IncludePath> includeDirectories): base(predecessorMessages)
        {
            SourceDirectories = sourceDirectories;
            IncludeDirectories = includeDirectories;
        }
        
        public IEnumerable<VirtualDirectory> SourceDirectories { get; }
        public IEnumerable<IncludePath> IncludeDirectories { get; }

        protected override string DataToString()
        {
            return $"{nameof(SourceDirectories)}: {string.Join(", ", SourceDirectories)}; {nameof(IncludeDirectories)}: {string.Join(", ", IncludeDirectories)}";
        }
    }
}
