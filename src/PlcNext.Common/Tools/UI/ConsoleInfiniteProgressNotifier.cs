#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Timers;

namespace PlcNext.Common.Tools.UI
{
    internal class ConsoleInfiniteProgressNotifier : ConsoleProgressNotifierBase
    {
        private readonly Action disposeAction;
        private readonly Timer updateTimer = new(Constants.ConsoleInfiniteProgressDotIntervalInMs) {AutoReset = true};

        public ConsoleInfiniteProgressNotifier(
            IUserInterface userInterface,
            string startMessage,
            Action disposeAction = null,
            IProgressNotifier parent = null)
            : base(userInterface, startMessage, parent)
        {
            this.disposeAction = disposeAction ?? (() => { });
            updateTimer.Elapsed += UpdateTimerOnElapsed;
            updateTimer.Start();
            
            WriteInfinite();
        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            SetOutputPrefix();
            UserInterface.WriteInformation(".", false);
        }

        public override void TickIncrement(double addedProgress = 1, string message = "") {}

        public override void Tick(double totalProgress, string message = "") {}

        public override void Dispose()
        {
            if (!wasDisposed)
            {
                updateTimer.Elapsed -= UpdateTimerOnElapsed;
                updateTimer.Dispose();
                WriteInfinite("Done");
                base.Dispose();
                Parent?.TickIncrement();
                disposeAction();
            }
        }
        
        private void WriteInfinite(string message = "")
        {
            string resultMessage = $"[Infinite] {StartMessage}";
            if (!string.IsNullOrEmpty(message))
            {
                resultMessage += $": {message}";

            }
            
            SetProgressPrefix();
            UserInterface.WriteInformation(resultMessage, true);
            SetOutputPrefix();
            
        }
    }
}
