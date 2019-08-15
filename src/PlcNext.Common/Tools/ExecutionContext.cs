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
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;

namespace PlcNext.Common.Tools
{
    public class ExecutionContext: IUserInterface, ILog
    {
        private readonly IUserInterface userInterface;
        private ChangeObservable observable;
        private readonly ILog log;

        public ExecutionContext(IUserInterface userInterface, ILog log)
        {
            this.userInterface = userInterface;
            this.log = log;
        }

        public ExecutionContext RedirectOutput(IUserInterface newUserInterface)
        {
            return new ExecutionContext(newUserInterface, log) {observable = observable};
        }

        public ChangeObservable Observable
        {
            get
            {
                if (observable == null)
                {
                    throw new InvalidOperationException("Observable is not available.");
                }

                return observable;
            }
            private set
            {
                if (observable != null && value != null)
                {
                    throw new InvalidOperationException("Observable cannot be set twice.");
                }

                observable = value;
            }
        }

        public event EventHandler<EventArgs> ObservableRegistered; 
        public event EventHandler<EventArgs> ObservableUnregistered; 

        public IDisposable RegisterObservable(ChangeObservable observable)
        {
            Observable = observable;
            OnObservableRegistered();
            return new DisposeAction(() =>
            {
                Observable = null;
                OnObservableUnregistered();
            });
        }

        public void WriteInformation(string message, bool showUser = true)
        {
            if (showUser)
            {
                userInterface.WriteInformation(message);
            }
            else
            {
                log.LogInformation(message);
            }
        }

        public void WriteVerbose(string message, bool showUser = true)
        {
            if (showUser)
            {
                userInterface.WriteVerbose(message);
            }
            else
            {
                log.LogVerbose(message);
            }
        }

        public void WriteError(string message, bool showUser = true)
        {
            if (showUser)
            {
                userInterface.WriteError(message);
            }
            else
            {
                log.LogError(message);
            }
        }

        public void WriteWarning(string message, bool showUser = true)
        {
            if (showUser)
            {
                userInterface.WriteWarning(message);
            }
            else
            {
                log.LogWarning(message);
            }
        }

        void IUserInterface.WriteInformation(string message)
        {
            WriteInformation(message);
        }

        void IUserInterface.WriteVerbose(string message)
        {
            WriteVerbose(message);
        }

        void IUserInterface.WriteError(string message)
        {
            WriteError(message);
        }

        public void SetVerbosity(bool verbose)
        {
            userInterface.SetVerbosity(verbose);
        }

        void IUserInterface.WriteWarning(string message)
        {
            WriteWarning(message);
        }

        public void PauseOutput()
        {
            userInterface.PauseOutput();
        }

        public void ResumeOutput()
        {
            userInterface.ResumeOutput();
        }

        public void SetQuiet(bool quiet)
        {
            userInterface.SetQuiet(quiet);
        }

        void ILog.LogVerbose(string message)
        {
            WriteVerbose(message, false);
        }

        void ILog.LogInformation(string message)
        {
            WriteInformation(message, false);
        }

        void ILog.LogWarning(string message)
        {
            WriteWarning(message, false);
        }

        void ILog.LogError(string message)
        {
            WriteError(message, false);
        }

        protected virtual void OnObservableRegistered()
        {
            ObservableRegistered?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnObservableUnregistered()
        {
            ObservableUnregistered?.Invoke(this, EventArgs.Empty);
        }
    }
}
