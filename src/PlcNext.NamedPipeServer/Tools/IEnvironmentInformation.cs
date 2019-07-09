#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.NamedPipeServer.Tools
{
    public interface IEnvironmentInformation
    {
        string InterProcessServerNameBase{get;}
        
        int CurrentProcessId { get; }
    }
}