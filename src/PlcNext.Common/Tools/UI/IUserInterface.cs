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
        void WriteInformation(string message, bool withNewLine = true);
        void WriteVerbose(string message, bool withNewLine = true);
        void WriteError(string message, bool withNewLine = true);
        void SetVerbosity(bool verbose);
        void WriteWarning(string message, bool withNewLine = true);
        void PauseOutput();
        void ResumeOutput();
        void SetQuiet(bool quiet);
        void SetPrefix(string prefix);
    }
}
