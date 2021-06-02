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
using System.Linq;
using Agents.Net;
using PlcNext.CppParser.CppRipper.Messages;
using PlcNext.CppParser.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(CppStreamParsedSuccessfully))]
    [Produces(typeof(IncludesFound))]
    internal class IncludesParser : Agent
    {
        public IncludesParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CppStreamParsedSuccessfully parsedSuccessfully = messageData.Get<CppStreamParsedSuccessfully>();
            OnMessage(new IncludesFound(messageData, GetIncludes()));
            
            string[] GetIncludes()
            {
                List<string> result = new List<string>();
                foreach (ParseNode includeNode in parsedSuccessfully.Root.GetHierarchy().Where(n => n.RuleType == "sequence" &&
                                                                                n.RuleName == "pp_directive" &&
                                                                                n.Any(c => c.ToString().Equals("include", StringComparison.OrdinalIgnoreCase))))
                {
                    ParseNode include = includeNode.FirstOrDefault(n => n.RuleName == "until_eol");
                    if (include != null)
                    {
                        result.Add(include.ToString().Trim('\"'));
                    }
                }

                return result.ToArray();
            }
        }
    }
}
