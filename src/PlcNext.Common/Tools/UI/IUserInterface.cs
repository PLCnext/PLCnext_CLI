#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Tools.UI
{
    public interface IUserInterface
    {
        void WriteInformation(string message);
        void WriteVerbose(string message);
        void WriteError(string message);
        void SetVerbosity(bool verbose);
        void WriteWarning(string message);
        void PauseOutput();
        void ResumeOutput();
        void SetQuiet(bool quiet);
    }
}
