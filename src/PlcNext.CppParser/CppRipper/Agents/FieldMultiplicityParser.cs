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
    [Consumes(typeof(IdentifiersParsed))]
    [Consumes(typeof(DataTypeParsed))]
    [Produces(typeof(MultiplicitiesParsed))]
    public class FieldMultiplicityParser : Agent
    {
        private readonly MessageCollector<IdentifiersParsed, DataTypeParsed> collector;
        public FieldMultiplicityParser(IMessageBoard messageBoard) : base(messageBoard)
        {
            collector = new MessageCollector<IdentifiersParsed, DataTypeParsed>(OnMessagesCollected);
        }

        private void OnMessagesCollected(MessageCollection<IdentifiersParsed, DataTypeParsed> set)
        {
            set.MarkAsConsumed(set.Message1);
            set.MarkAsConsumed(set.Message2);

            int[][] parsedMultiplicities = set.Message1.Identifiers.Except(set.Message2.TypeNodes)
                                              .Select(GetMultiplicity)
                                              .ToArray();
            OnMessage(new MultiplicitiesParsed(set, parsedMultiplicities));

            int[] GetMultiplicity(ParseNode identifier)
            {
                identifier = identifier.GetParent();
                identifier = identifier.SkipUnnamedParents();
                int index = identifier.GetParentIndex();
                ParseNode parent = identifier.GetParent();
                List<int> multiplicities = new List<int>();
                bool firstMultiplicityFound = false;
                foreach (ParseNode sibling in parent.Skip(index+1).SkipUnnamed())
                {
                    if (sibling.RuleType != "choice" || sibling.RuleName != "node")
                    {
                        continue;
                    }

                    ParseNode child = sibling.FirstOrDefault();
                    if (child?.RuleType == "sequence" && child.RuleName == "bracketed_group")
                    {
                        string bracketGroup = child.ToString().Trim();
                        if (int.TryParse(bracketGroup.Substring(1, bracketGroup.Length-2), out int result))
                        {
                            multiplicities.Add(result);
                            firstMultiplicityFound = true;
                        }
                    }
                    else if(firstMultiplicityFound)
                    {
                        break;
                    }
                }

                return multiplicities.ToArray();
            }
        }

        protected override void ExecuteCore(Message messageData)
        {
            collector.Push(messageData);
        }
    }
}
