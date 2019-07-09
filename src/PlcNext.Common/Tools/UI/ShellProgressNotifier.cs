#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using ShellProgressBar;

namespace PlcNext.Common.Tools.UI
{
    internal class ShellProgressNotifier: ShellProgressNotifierBase
    {
        private readonly double maxTicks;
        private readonly Action disposeAction;

        public override void Dispose()
        {
            base.Dispose();
            disposeAction();
        }

        public ShellProgressNotifier(double maxTicks, string startMessage, ProgressBarOptions options, Action disposeAction = null) : base(maxTicks, startMessage, options)
        {
            this.maxTicks = maxTicks;
            this.disposeAction = disposeAction;
        }

        protected override ProgressBarBase CreateProgressBar()
        {
            return new ProgressBar((int) (maxTicks * Scaling), StartMessage, Options);
        }
    }
}
