#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Test.PlcNext.Tools.Abstractions
{
    internal interface IProcessManagerAbstraction : IAbstraction
    {
        bool ThrowError { get; set; }
        bool CommandExecuted(string command, params string[] args);
        string GetLastCommandArgs(string executable);
        void WithOtherProgramInstance(int processId);
    }
}
