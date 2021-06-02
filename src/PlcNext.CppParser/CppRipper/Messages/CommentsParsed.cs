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

namespace PlcNext.CppParser.CppRipper.Messages
{
    internal class CommentsParsed : Message
    {
        public CommentsParsed(Message predecessorMessage, IReadOnlyCollection<CppComment> comments): base(predecessorMessage)
        {
            Comments = comments;
        }

        public CommentsParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CppComment> comments): base(predecessorMessages)
        {
            Comments = comments;
        }
        
        public IReadOnlyCollection<CppComment> Comments { get; }

        protected override string DataToString()
        {
            return $"{nameof(Comments)}: {string.Join(", ",Comments)}";
        }
    }
}
