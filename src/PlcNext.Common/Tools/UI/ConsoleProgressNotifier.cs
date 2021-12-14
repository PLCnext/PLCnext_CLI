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
    internal class ConsoleProgressNotifier: ConsoleProgressNotifierBase
    {
        private readonly double maxTicks;
        private readonly Action disposeAction;
        private double currentTicks;

        public ConsoleProgressNotifier(
            IUserInterface userInterface,
            double maxTicks,
            string startMessage,
            Action disposeAction = null,
            IProgressNotifier parent = null)
            : base(userInterface, startMessage, parent)
        {
            this.maxTicks = maxTicks;
            this.disposeAction = disposeAction ?? (() => { });
            currentTicks = 0;
            WriteProgress();
        }

        public override void TickIncrement(double addedProgress = 1, string message = "")
        {
            currentTicks += addedProgress;
            WriteProgress(message);
            if (currentTicks >= maxTicks)
            {
                Dispose();
            }
        }

        public override void Tick(double totalProgress, string message = "")
        {
            currentTicks = totalProgress;
            WriteProgress(message);
            if (currentTicks >= maxTicks)
            {
                Dispose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Parent?.TickIncrement();
            disposeAction();
        }
        
        private void WriteProgress(string message = null)
        {
            string progress = $"{(currentTicks / maxTicks)*100:0.##}" + "%";
            string resultMessage = $"[{progress}] {StartMessage}";
            if (!string.IsNullOrEmpty(message))
            {
                resultMessage += $": {message}";
            }
            
            SetProgressPrefix();
            UserInterface.WriteInformation(resultMessage);
            SetOutputPrefix();
        }
    }
}
