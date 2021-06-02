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
    internal class FieldSetCommentsParsed : Message
    {
        public FieldSetCommentsParsed(Message predecessorMessage, IReadOnlyCollection<CppComment> comments): base(predecessorMessage)
        {
            Comments = comments;
        }

        public FieldSetCommentsParsed(IEnumerable<Message> predecessorMessages, IReadOnlyCollection<CppComment> comments): base(predecessorMessages)
        {
            Comments = comments;
        }
        
        public IReadOnlyCollection<CppComment> Comments { get; }

        protected override string DataToString()
        {
            return $"{nameof(Comments)}: {Comments}";
        }
    }
}
