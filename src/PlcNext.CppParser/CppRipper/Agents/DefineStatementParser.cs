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
using System.Text.RegularExpressions;
using Agents.Net;
using PlcNext.CppParser.Messages;
using PlcNext.CppParser.CppRipper.Messages;

namespace PlcNext.CppParser.CppRipper.Agents
{
    [Consumes(typeof(CppStreamParsedSuccessfully))]
    [Produces(typeof(DefineStatementsParsed))]
    internal class DefineStatementParser : Agent
    {
        public static readonly Regex StatementParser = new Regex(@"^(?<Key>[^\s\(\)]+)\s+(?<Value>.+?)(?:\s*\/\/.*)?$",
                                                                  RegexOptions.Compiled);
        public DefineStatementParser(IMessageBoard messageBoard) : base(messageBoard)
        {
        }

        protected override void ExecuteCore(Message messageData)
        {
            CppStreamParsedSuccessfully parsedSuccessfully = messageData.Get<CppStreamParsedSuccessfully>();
            Dictionary<string, string> directives = new Dictionary<string, string>();
            foreach (ParseNode defineNode in parsedSuccessfully.Root.GetHierarchy().Where(n => n.RuleType == "sequence" &&
                n.RuleName == "pp_directive" &&
                n.Any(c => c.ToString().Equals("define", StringComparison.OrdinalIgnoreCase))))
            {
                string statement = defineNode.FirstOrDefault(n => n.RuleName == "until_eol")?.ToString() ??
                                   string.Empty;
                Match match = StatementParser.Match(statement);
                string key = match.Groups["Key"].Value.Trim();
                if (match.Success && !directives.ContainsKey(key))
                {
                    directives.Add(match.Groups["Key"].Value, match.Groups["Value"].Value.Trim());
                }
            }
            OnMessage(new DefineStatementsParsed(messageData, directives));
        }
    }
}
