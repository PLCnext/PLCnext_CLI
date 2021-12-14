#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Linq;
using System.Text.RegularExpressions;
using Agents.Net;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Parser;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Produces(typeof(DeclarationInvalid))]
    [Intercepts(typeof(DeclarationFound))]
    [Intercepts(typeof(IdentifiersParsed))]
    [Intercepts(typeof(DataTypeParsed))]
    internal class DeclarationFilter : InterceptorAgent
    {
        private readonly MessageCollector<IdentifiersParsed, DataTypeParsed> collector = new MessageCollector<IdentifiersParsed, DataTypeParsed>();
        public DeclarationFilter(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override InterceptionAction InterceptCore(Message messageData)
        {
            if (messageData.TryGet(out DeclarationFound declarationFound))
            {
                if (declarationFound.Declaration.GetHierarchy().Any(n => IsForbiddenParanthesisGroup(n)) ||
                    declarationFound.Declaration.GetHierarchy().Any(n => n.RuleName == "typedef_decl") ||
                    declarationFound.Declaration.GetHierarchy().Any(n => n.RuleName == "pp_directive"))
                {
                    OnMessage(DeclarationInvalid.Create(messageData));
                    return InterceptionAction.DoNotPublish;
                }

                return InterceptionAction.Continue;
            }

            if(messageData.TryGet(out IdentifiersParsed identifiersParsed))
            {
                collector.Push(identifiersParsed);
                if (identifiersParsed.Identifiers.FirstOrDefault()?.ToString() == "using")
                {
                    OnMessage(DeclarationInvalid.Create(messageData));
                    return InterceptionAction.DoNotPublish;
                }
                
                return InterceptionAction.Continue;
            }

            if (messageData.Is<DataTypeParsed>())
            {
                bool continueExecution = true;
                collector.PushAndExecute(messageData, set =>
                {
                    set.MarkAsConsumed(set.Message1);
                    set.MarkAsConsumed(set.Message2);
                    if (set.Message1.Identifiers.SequenceEqual(set.Message2.TypeNodes))
                    {
                        OnMessage(CreateParserErrorMessage(set.Message2));
                        continueExecution = false;
                    }
                });
                return continueExecution ? InterceptionAction.Continue : InterceptionAction.DoNotPublish;
            }
            
            return InterceptionAction.Continue;

            DeclarationInvalid CreateParserErrorMessage(DataTypeParsed dataTypeParsed)
            {
                if (dataTypeParsed.TypeNodes.Any())
                {
                    (int line, int column) position = dataTypeParsed.Declaration.Position;
                    ParserMessage message = new ParserMessage("CPP0001", position.line, position.column);
                    return DeclarationInvalid.Create(dataTypeParsed, message);
                }
                return DeclarationInvalid.Create(dataTypeParsed);
            }
			
			bool IsForbiddenParanthesisGroup(ParseNode n)
            {
                if (n.RuleName != "paran_group")
                {
                    return false;
                }

                ParseNode parent = n.GetParent();
                while (parent.Count(c => c.RuleName != "comment_set") == 1)
                {
                    n = parent;
                    parent = n.GetParent();
                }
                
                return parent.TakeWhile(c => c != n).All(c => !FieldParser.EqualsMatch.IsMatch(c.ToString()));
            }
        }
    }
}
