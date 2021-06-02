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
using PlcNext.Common.Tools.SDK;

namespace PlcNext.CppParser.IncludeManager.Messages
{
    public class IncludeDirectoriesFound : Message
    {
        public IncludeDirectoriesFound(Message predecessorMessage, IEnumerable<IncludePath> includeDirectories): base(predecessorMessage)
        {
            IncludeDirectories = includeDirectories;
        }

        public IncludeDirectoriesFound(IEnumerable<Message> predecessorMessages, IEnumerable<IncludePath> includeDirectories): base(predecessorMessages)
        {
            IncludeDirectories = includeDirectories;
        }
        
        public IEnumerable<IncludePath> IncludeDirectories { get; } 

        protected override string DataToString()
        {
            return $"{nameof(IncludeDirectories)}: {IncludeDirectories}";
        }
    }
}
