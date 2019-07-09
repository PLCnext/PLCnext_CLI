#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;

namespace PlcNext.Common.CommandLine
{
    public interface ICommandLineParser
    {
        Task<int> Parse(params string[] args);

        string GetParseResult(params string[] args);
    }
}
