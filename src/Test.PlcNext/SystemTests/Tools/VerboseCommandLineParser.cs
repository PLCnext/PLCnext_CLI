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
using System.Text;
using System.Threading.Tasks;
using PlcNext.Common.CommandLine;

namespace Test.PlcNext.SystemTests.Tools
{
    internal class VerboseCommandLineParser : ICommandLineParser
    {
        private readonly ICommandLineParser commandLineParserImplementation;

        public VerboseCommandLineParser(ICommandLineParser commandLineParserImplementation)
        {
            this.commandLineParserImplementation = commandLineParserImplementation;
        }

        public Task<int> Parse(params string[] args)
        {
            if (!args.Contains("--verbose"))
            {
                args = args.Concat(new[] {"--verbose"}).ToArray();
            }
            return commandLineParserImplementation.Parse(args);
        }

        public string GetParseResult(params string[] args)
        {
            if (!args.Contains("--verbose"))
            {
                args = args.Concat(new[] { "--verbose" }).ToArray();
            }
            return commandLineParserImplementation.GetParseResult(args);
        }
    }
}
