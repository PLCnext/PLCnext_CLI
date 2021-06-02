#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(TypeDeclarationFound))]
    [Produces(typeof(CommentsParsed))]
    internal class CommentParser : Agent
    {
        public CommentParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            TypeDeclarationFound typeDeclarationFound = messageData.Get<TypeDeclarationFound>();
            CppComment[] comments = typeDeclarationFound.Content.ChildrenSkipUnnamed()
                                                        .Where(IsComment)
                                                        .Where(c => !string.IsNullOrEmpty(c.ToString()))
                                                        .Select(CppComment.Parse)
                                                        .ToArray();
            OnMessage(new CommentsParsed(messageData, comments));

            bool IsComment(ParseNode node)
            {
                return node.RuleType == "sequence" && node.RuleName == "comment_set";
            }
        }
    }
}
