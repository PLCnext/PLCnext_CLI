#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(TypeDeclarationFound))]
    [Produces(typeof(DeclarationFound))]
    [Produces(typeof(NoDeclarationFound))]
    internal class DeclarationFinder : Agent
    {
        public DeclarationFinder(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            if (messageData.Is<EnumFound>())
            {
                //No field parsing for enums
                OnMessage(NoDeclarationFound.Create(messageData));
                return;
            }
            
            TypeDeclarationFound declarationFound = messageData.Get<TypeDeclarationFound>();
            
            ParseNode list = declarationFound.Content.GetDeclarationList(declarationFound.Declaration);
            if (list == null)
            {
                OnMessage(NoDeclarationFound.Create(messageData));
                return;
            }

            IReadOnlyCollection<Message> messages = list.ChildrenSkipUnnamed()
                                                        .Where(n => n.RuleName == "declaration")
                                                        .Select((declaration, index) => new DeclarationFound(messageData, declaration, index))
                                                        .ToArray();
            if (messages.Any())
            {
                OnMessages(messages);
            }
            else
            {
                OnMessage(NoDeclarationFound.Create(messageData));
            }
        }
    }
}
