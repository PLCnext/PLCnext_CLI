#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PlcNext.Common.CodeModel;

namespace PlcNext.CppParser.CppRipper.CodeModel
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CppComment : IComment
    {
        private CppComment(CodePosition position, string content)
        {
            Position = position;
            Content = content;
        }

        public static IComment Parse(ParseNode commentNode)
        {
            return new CppComment(new CodePosition(commentNode.Position.line, commentNode.Position.column), commentNode.ToString());
        }

        public CodePosition Position { get; }
        public string Content { get; }
    }
}
