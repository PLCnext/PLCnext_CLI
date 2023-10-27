#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Commands
{
    public class CheckProjectCommandArgs : CommandArgs
    {
        public CheckProjectCommandArgs(string path)
        {
            Path = path; 
        }
        public string Path { get; }
    }
}
