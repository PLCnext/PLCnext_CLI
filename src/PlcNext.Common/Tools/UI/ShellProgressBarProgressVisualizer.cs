#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using ShellProgressBar;

namespace PlcNext.Common.Tools.UI
{
    internal class ShellProgressBarProgressVisualizer : IProgressVisualizer
    {
        private readonly ProgressBarOptions defaultOptions = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.White,
            BackgroundColor = ConsoleColor.DarkGray,
            ProgressCharacter = '─',
            BackgroundCharacter = '─',
            CollapseWhenFinished = true
        };

        private readonly IUserInterface userInterface;

        public ShellProgressBarProgressVisualizer(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        public IProgressNotifier Spawn(double maxTicks, string startMessage, string completedMessage)
        {
            userInterface.PauseOutput();
            return new ShellProgressNotifier(maxTicks, startMessage, defaultOptions, DisposeAction);

            void DisposeAction()
            {
                userInterface.ResumeOutput();
                if (!string.IsNullOrEmpty(completedMessage))
                {
                    userInterface.WriteInformation(completedMessage);
                }
            }
        }

        public IDisposable SpawnInfiniteProgress(string startMessage)
        {
            userInterface.PauseOutput();
            return new ShellInfinitProgressNotifier(startMessage, defaultOptions, DisposeAction);

            void DisposeAction()
            {
                userInterface.ResumeOutput();
            }
        }
    }
}
