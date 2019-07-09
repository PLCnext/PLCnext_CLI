#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Diagnostics;

namespace PlcNext.NamedPipeServer.Tools
{
    public class RealEnvironmentInformation : IEnvironmentInformation
    {
        public string InterProcessServerNameBase => "d50bfe87-16fb-48dd-a348-10b0eeb4e7c6-";
        public int CurrentProcessId => Process.GetCurrentProcess().Id;
    }
}