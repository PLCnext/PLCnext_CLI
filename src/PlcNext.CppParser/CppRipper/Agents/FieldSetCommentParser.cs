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
    [Consumes(typeof(DeclarationFound))]
    [Produces(typeof(FieldSetCommentsParsed))]
    internal class FieldSetCommentParser : Agent
    {
        public FieldSetCommentParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            DeclarationFound declarationFound = messageData.Get<DeclarationFound>();
            
            OnMessage(new FieldSetCommentsParsed(messageData, GetComments()));
            
            CppComment[] GetComments()
            {
                ParseNode content = declarationFound.Declaration.ChildrenSkipUnnamed().First(n => n.RuleType == "plus" && n.RuleName == "declaration_content");
                return content.ChildrenSkipUnnamed().ToArray()
                              .SkipAfterLastVisibilityGroup()
                              .Where(IsComment)
                              .Where(c => !string.IsNullOrEmpty(c.ToString()))
                              .Select(CppComment.Parse)
                              .ToArray();

                bool IsComment(ParseNode node)
                {
                    return node.RuleType == "sequence" && node.RuleName == "comment_set";
                }
            }
        }
    }
}
