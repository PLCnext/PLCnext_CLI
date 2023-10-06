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
        private IUserInterface userInterface;
        private ChangeObservable observable;
        private readonly ILog log;

        public ExecutionContext(IUserInterface userInterface, ILog log)
        {
            this.userInterface = userInterface;
            this.log = log;
        }

        public ExecutionContext WithRedirectedOutput(IUserInterface newUserInterface)
        {
            return new ExecutionContext(newUserInterface, log) {observable = observable};
        }

        public IDisposable RedirectOutput(IUserInterface newUserInterface)
        {
            IUserInterface original = userInterface;
            userInterface = newUserInterface;
            return new DisposeAction(() => userInterface = original);
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

        public void WriteInformation(string message, bool showUser = true, bool withNewLine = true)
        {
            if (showUser)
            {
                userInterface.WriteInformation(message, withNewLine);
            }
            else
            {
                log.LogInformation(message);
            }
        }

        public void WriteVerbose(string message, bool showUser = true, bool withNewLine = true)
        {
            if (showUser)
            {
                userInterface.WriteVerbose(message, withNewLine);
            }
            else
            {
                log.LogVerbose(message);
            }
        }

        public void WriteError(string message, bool showUser = true, bool withNewLine = true)
        {
            if (showUser)
            {
                userInterface.WriteError(message, withNewLine);
            }
            else
            {
                log.LogError(message);
            }
        }

        public void WriteWarning(string message, bool showUser = true, bool withNewLine = true)
        {
            if (showUser)
            {
                userInterface.WriteWarning(message, withNewLine);
            }
            else
            {
                log.LogWarning(message);
            }
        }

        void IUserInterface.WriteInformation(string message, bool withNewLine)
        {
            WriteInformation(message, withNewLine: withNewLine);
        }

        void IUserInterface.WriteVerbose(string message, bool withNewLine)
        {
            WriteVerbose(message, withNewLine: withNewLine);
        }

        void IUserInterface.WriteError(string message, bool withNewLine)
        {
            WriteError(message, withNewLine: withNewLine);
        }

        public void SetVerbosity(bool verbose)
        {
            userInterface.SetVerbosity(verbose);
        }

        void IUserInterface.WriteWarning(string message, bool withNewLine)
        {
            WriteWarning(message, withNewLine: withNewLine);
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

        public void SetPrefix(string prefix)
        {
            userInterface.SetPrefix(prefix);
        }

        public void LogVerbose(string message)
        {
            WriteVerbose(message, false);
        }

        public void LogInformation(string message)
        {
            WriteInformation(message, false);
        }

        public void LogWarning(string message)
        {
            WriteWarning(message, false);
        }

        public void LogError(string message)
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
