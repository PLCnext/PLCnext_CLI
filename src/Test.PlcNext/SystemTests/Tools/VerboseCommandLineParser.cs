#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools;

namespace Test.PlcNext.SystemTests.Tools
{
    internal class VerboseCommandLineParser : ICommandLineParser
    {
        private readonly ICommandLineParser commandLineParserImplementation;
        private readonly IExceptionHandler exceptionHandler;

        public VerboseCommandLineParser(ICommandLineParser commandLineParserImplementation,
                                        IExceptionHandler exceptionHandler)
        {
            this.commandLineParserImplementation = commandLineParserImplementation;
            this.exceptionHandler = exceptionHandler;
        }

        public async Task<int> Parse(params string[] args)
        {
            if (!args.Contains("--verbose"))
            {
                args = args.Concat(new[] {"--verbose"}).ToArray();
            }
            try
            {
                return await commandLineParserImplementation.Parse(args);
            }catch(Exception e)
            {
                if (!exceptionHandler.HandleException(e))
                {
                    throw;
                }

                return -1;
            }
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
