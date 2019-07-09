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

namespace PlcNext.Common.Commands
{
    public class CommandResult
    {
        public CommandResult(int externalResult, object detailedResult, IEnumerable<Exception> exceptions= null)
        {
            ExternalResult = externalResult;
            DetailedResult = detailedResult;
            Exceptions = exceptions ?? Enumerable.Empty<Exception>();
        }

        public int ExternalResult { get; }
        public object DetailedResult { get; }
        public IEnumerable<Exception> Exceptions { get; }
    }
}