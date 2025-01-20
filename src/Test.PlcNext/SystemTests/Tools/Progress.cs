#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Test.PlcNext.SystemTests.Tools
{
    public class Progress
    {
        public Progress(int currentProgress, int progressMaximum)
        {
            CurrentProgress = currentProgress;
            ProgressMaximum = progressMaximum;
            Message = "";
        }
        public Progress(int currentProgress, int progressMaximum, string message)
        {
            CurrentProgress = currentProgress;
            ProgressMaximum = progressMaximum;
            Message = message;
        }

        public int CurrentProgress { get; }
        public int ProgressMaximum { get; }
        public string Message { get; }

        public override string ToString()
        {
            return $"{nameof(CurrentProgress)}: {CurrentProgress}, {nameof(ProgressMaximum)}: {ProgressMaximum}, {nameof(Message)}: {Message}";
        }
    }
}
