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
        private Action<string> printMessage;

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            this.printMessage = printMessage;
            exportProvider.AddInstance(userInterface);
        }

        public MockedUserInterfaceAbstraction()
        {
            userInterface.WriteError(Arg.Do<string>(message =>
            {
                errors.Add(message);
                printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: ERROR: {message}");
            }));
            userInterface.WriteInformation(Arg.Do<string>(message =>
            {
                informations.Add(message);
                printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: INFO: {message}");
            }));
            userInterface.WriteVerbose(Arg.Do<string>(message =>
            {
                printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: VERBOSE: {message}");
            }));
            userInterface.WriteWarning(Arg.Do<string>(message =>
            {
                warnings.Add(message);
                printMessage?.Invoke($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: WARNING: {message}");
            }));
        }
        
        public IEnumerable<string> Errors => errors;
        public IEnumerable<string> Informations => informations;
        public IEnumerable<string> Warnings => warnings;
        
        public void Dispose()
        {
        }
    }
}
