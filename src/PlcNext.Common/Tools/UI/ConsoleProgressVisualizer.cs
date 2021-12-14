#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.UI
{
    internal class ConsoleProgressVisualizer : IProgressVisualizer
    {
        private readonly IUserInterface userInterface;

        public ConsoleProgressVisualizer(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        public IProgressNotifier Spawn(double maxTicks, string startMessage, string completedMessage)
        {
            return new ConsoleProgressNotifier(userInterface, maxTicks, startMessage, DisposeAction);

            void DisposeAction()
            {
                if (!string.IsNullOrEmpty(completedMessage))
                {
                    userInterface.SetPrefix(String.Empty);
                    userInterface.WriteInformation(completedMessage);
                }
            }
        }

        public IDisposable SpawnInfiniteProgress(string startMessage)
        {
            return new ConsoleInfiniteProgressNotifier(userInterface, startMessage, DisposeAction);
        
            void DisposeAction()
            {
            }
        }
    }
}
