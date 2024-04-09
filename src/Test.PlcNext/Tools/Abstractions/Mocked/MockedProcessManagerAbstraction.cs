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
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Core;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedProcessManagerAbstraction : IProcessManagerAbstraction
    {
        private class Command
        {
            public string Executable { get; }
            public string Arguments { get; }

            public Command(string executable, string arguments)
            {
                Executable = executable;
                Arguments = arguments;
            }

            public override string ToString()
            {
                return $"{nameof(Executable)}: {Executable}, {nameof(Arguments)}: {Arguments}";
            }
        }

        private readonly IProcessManager processManager = Substitute.For<IProcessManager>();

        public bool ThrowError { get; set; }

        public string ExitWithErrorForCommand { get; set; }

        public bool CommandExecuted(string command, params string[] args)
        {
            return executedCommands.Any(c => c.Executable.Contains(command) && args.All(c.Arguments.Contains));
        }

        public string GetLastCommandArgs(string executable)
        {
            Command executedCommand = executedCommands.FirstOrDefault(c => c.Executable.Contains(executable));
            executedCommand.Should().NotBeNull(
                $"command {executable} was expected to be executed. Available commands are:{Environment.NewLine}" +
                $"{string.Join(Environment.NewLine, executedCommands.Select(c => c.ToString()))}");
            return executedCommand.Arguments;
        }

        public void WithOtherProgramInstance(int processId)
        {
            processManager.GetOtherInstancesProcessIds().Returns(new[] {processId});
        }

        private readonly HashSet<Command> executedCommands = new HashSet<Command>();

        public MockedProcessManagerAbstraction()
        {
            ThrowError = false;
            ExitWithErrorForCommand = string.Empty;
            // mock the failure of a process
            processManager.StartProcess(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IUserInterface>(), Arg.Any<string>(), showOutput: Arg.Any<bool>(), showError: Arg.Any<bool>(), killOnDispose: Arg.Any<bool>())
                          .Returns(callinfo => MockProcess(callinfo));
            processManager.StartProcessWithSetup(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IUserInterface>(), Arg.Any<string>(),
                                                 Arg.Any<string>(), showOutput: Arg.Any<bool>(), showError: Arg.Any<bool>(),
                                                 killOnDispose: Arg.Any<bool>())
                           .Returns(callinfo => MockProcess(callinfo));

            IProcess MockProcess(CallInfo callinfo)
            {
                if (ThrowError && callinfo.ArgAt<string>(0) != "which" && callinfo.ArgAt<string>(0) != "where")
                {
                    ThrowError = false;
                    throw new FormattableException("mock process throws exception");
                }

                executedCommands.Add(new Command(callinfo.ArgAt<string>(0), callinfo.ArgAt<string>(1)));
                ProcessCommand(callinfo.ArgAt<string>(1), callinfo.Arg<IUserInterface>());
                IProcess process = Substitute.For<IProcess>();
                if (((callinfo.ArgAt<string>(0) == "which" ||
                     callinfo.ArgAt<string>(0) == "where") &&
                    !callinfo.ArgAt<string>(1).Contains("cmake")) ||
                     (!string.IsNullOrEmpty(ExitWithErrorForCommand) && callinfo.ArgAt<string>(0).EndsWith(ExitWithErrorForCommand)))
                {
                    process.ExitCode.Returns(-1);
                }
                else
                {
                    process.ExitCode.Returns(0);
                }
                return process;
            }
            void ProcessCommand(string command, IUserInterface userInterface)
            {
                if (command == "get setting --all" || command == "get setting -a")
                {
                    userInterface.WriteInformation(@"{""AttributePrefix"": ""#"",  ""FancySetting"": [""a"", ""b""]}");
                }
                if (command.Contains("-target:GetProjectVariables"))
                {
                    userInterface.WriteInformation("@begineclrversioneCLR3.4@endeclrversion");
                    userInterface.WriteInformation("@beginoutputpathCli@endoutputpath");
                }
            }
        }

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            exportProvider.AddInstance(processManager);
        }

        public void Dispose()
        {
        }
        
    }
}
