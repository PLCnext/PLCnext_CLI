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
using System.Diagnostics;
using NSubstitute;
using PlcNext.Common.Tools.UI;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedUserInterfaceAbstraction : IUserInterfaceAbstraction
    {
        private readonly IUserInterface userInterface = Substitute.For<IUserInterface>();
        private List<string> errors = new List<string>();
        private List<string> informations = new List<string>();
        private List<string> warnings = new List<string>();

        public void Initialize(InstancesRegistrationSource exportProvider)
        {
            exportProvider.AddInstance(userInterface);
        }

        public MockedUserInterfaceAbstraction()
        {
            userInterface.WriteError(Arg.Do<string>(message =>
            {
                errors.Add(message);
                Trace.TraceError($"ERROR: {message}");
            }));
            userInterface.WriteInformation(Arg.Do<string>(message =>
            {
                informations.Add(message);
                Trace.TraceInformation($"INFO: {message}");
            }));
            userInterface.WriteVerbose(Arg.Do<string>(message =>
            {
                Trace.TraceInformation($"VERBOSE: {message}");
            }));
            userInterface.WriteWarning(Arg.Do<string>(message =>
            {
                warnings.Add(message);
                Trace.TraceWarning($"WARNING: {message}");
            }));
        }
        
        public IEnumerable<string> Errors => errors;
        public IEnumerable<string> Informations => informations;
        public IEnumerable<string> Warnings => warnings;

        private ActionTraceListener activeTraceListener;
        public void RegisterOutputPrinter(Action<string> printer)
        {
            activeTraceListener = new ActionTraceListener(printer);
            Trace.Listeners.Add(activeTraceListener);
        }

        private class ActionTraceListener : TraceListener
        {
            private readonly Action<string> printer;

            public ActionTraceListener(Action<string> printer)
            {
                this.printer = printer;
            }

            public override void Write(string message)
            {
                //do nothing usually only header information
            }

            public override void WriteLine(string message)
            {
                printer(message);
            }
        }

        public void Dispose()
        {
            Trace.Listeners.Remove(activeTraceListener);
            activeTraceListener?.Dispose();
        }
    }
}
