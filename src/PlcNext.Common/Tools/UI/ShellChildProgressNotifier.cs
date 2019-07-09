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
using ShellProgressBar;

namespace PlcNext.Common.Tools.UI
{
    internal class ShellChildProgressNotifier : ShellProgressNotifierBase
    {
        private readonly double maxTicks;
        private readonly ShellProgressNotifierBase parent;

        public ShellChildProgressNotifier(double maxTicks, string startMessage, ProgressBarOptions options, ShellProgressNotifierBase parent) : base(maxTicks, startMessage, options)
        {
            this.maxTicks = maxTicks;
            this.parent = parent;
            parent.Disposed += ParentOnDisposed;
        }

        private void ParentOnDisposed(object sender, EventArgs e)
        {
            parent.Disposed -= ParentOnDisposed;
            Dispose();
        }

        protected override ProgressBarBase CreateProgressBar()
        {
            return parent.ProgressBar.Spawn((int)(maxTicks * Scaling),StartMessage, Options);
        }

        public override void Dispose()
        {
            base.Dispose();
            parent.TickIncrement();
        }
    }
}
