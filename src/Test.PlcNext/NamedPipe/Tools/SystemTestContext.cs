#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using PlcNext.Common.CommandLine;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer;
using PlcNext.NamedPipeServer.CommandLine;
using PlcNext.NamedPipeServer.Communication;
using PlcNext.NamedPipeServer.Tools;
using Test.PlcNext.SystemTests.Tools;
using Xunit;

namespace Test.PlcNext.NamedPipe.Tools
{
    public class SystemTestContext : IDisposable
    {
        private IContainer host;
        private ICliServer server;
        private IClientSimulator clientSimulator;
        private readonly AutoResetEvent commandResetEvent = new AutoResetEvent(false);
        //Executed commands - Cancel on token source completes the command
        private readonly IssuedCommands issuedCommands = new IssuedCommands();
        private readonly ManualResetEvent disconnectedResetEvent = new ManualResetEvent(false);

        public bool Initialized { get; private set; }

        private ILog Log => Host.Resolve<ILog>();

        private IContainer Host
        {
            get
            {
                CheckInitialized();
                return host;
            }
            set => host = value;
        }

        private void CheckInitialized()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Test context not initialized");
            }
        }

        protected IClientSimulator ClientSimulator
        {
            get
            {
                CheckInitialized();
                if (clientSimulator == null)
                {
                    throw new InvalidOperationException("The server user interface was not initialized.");
                }

                return clientSimulator;
            }
        }

        public async Task StartServer(bool heartbeat)
        {
            server = Host.Resolve<ICliServer>();
            server.Disconnected += ServerOnDisconnected;
            string serverName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                    ? Guid.NewGuid().ToByteString()
                                    : $"/tmp/{Guid.NewGuid().ToByteString()}";
            Task<bool> serverStart = server.Start(serverName, heartbeat);
            clientSimulator = NamedPipeCommunicationProtocolSimulator.Connect(serverName,Host.Resolve<StreamFactory>(), Host.Resolve<ILog>());
            Assert.True(await serverStart, "Server not started correctly. See log.");
        }

        private void ServerOnDisconnected(object sender, EventArgs e)
        {
            disconnectedResetEvent.Set();
        }

        public async Task SendHandshake(Version handshakeProtocolVersion, int count = 1)
        {
            if (handshakeProtocolVersion == null)
            {
                return;
            }
            string message = $"{{\"type\":\"handshake\", \"protocolVersion\":{{\"major\":{handshakeProtocolVersion.Major}, \"minor\":{handshakeProtocolVersion.Minor}}}}}";
            await ClientSimulator.WriteMessage(message, count:count);
        }

        public void Initialize(Action<string> printMessage)
        {
            ContainerBuilder builder = new ContainerBuilder();
            ILog log = new LogTracer(printMessage);
            builder.RegisterModule(new BaseDiModule());
            RegisterCommandLineBuilder(builder, printMessage);
            RegisterEnvironmentInformation(builder);
            builder.RegisterInstance(log);
            Host = builder.Build();
            
            Initialized = true;
        }

        protected virtual void RegisterEnvironmentInformation(ContainerBuilder builder)
        {
            builder.RegisterInstance(Substitute.For<IEnvironmentInformation>());
        }

        protected virtual void RegisterCommandLineBuilder(ContainerBuilder builder, Action<string> printMessage)
        {
            builder.RegisterInstance(CreateCommandLineBuilder());
            
            ICommandLineBuilder CreateCommandLineBuilder()
            {
                ICommandLineBuilder commandLineBuilder = Substitute.For<ICommandLineBuilder>();
                commandLineBuilder.BuildCommandLineInstance(null, null, null, default(CancellationToken))
                                  .ReturnsForAnyArgs((info) => CreateParser(info.Arg<CancellationToken>(),
                                                                            info.Arg<IUserInterface>(),
                                                                            info.Arg<IProgressVisualizer>()));
                return commandLineBuilder;

                IDisposableCommandLineParser CreateParser(CancellationToken token, IUserInterface @interface,
                                                          IProgressVisualizer visualizer)
                {
                    IDisposableCommandLineParser commandLineParser = Substitute.For<IDisposableCommandLineParser>();
                    commandLineParser.Parse(null).ReturnsForAnyArgs((info) =>
                    {
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        //Don't use source in tasks as it should not throw exceptions
                        Task<int> commandExecution = ExecuteCommand(info.Arg<string[]>(), tokenSource.Token);
                        issuedCommands.Add(info.Arg<string[]>(), tokenSource, token, @interface, visualizer);
                        commandResetEvent.Set();
                        return commandExecution;
                    });
                    commandLineParser.GetParseResult(null)
                                     .ReturnsForAnyArgs((info) => "{\"error\":\"This is a test double\"}");
                    return commandLineParser;

                    async Task<int> ExecuteCommand(string[] args, CancellationToken cancellationToken)
                    {
                        while (!cancellationToken.IsCancellationRequested && !token.IsCancellationRequested)
                        {
                            await Task.Delay(10);
                        }

                        return issuedCommands.GetResult(args);
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            server?.StopServer();
            host?.Dispose();
            commandResetEvent.Dispose();
            issuedCommands.Dispose();
            disconnectedResetEvent.Dispose();
        }

        public async Task SendCommand(string command, int count = 1)
        {
            string message = $"{{\"type\":\"command\", \"command\":\"{command}\"}}";
            await ClientSimulator.WriteMessage(message, count:count);
        }

        public void CheckCommandTriggered(string[] command, int count = -1)
        {
            if (!command.Any())
            {
                commandResetEvent.WaitOne(NamedPipeCommunicationProtocolSimulator.DefaultTimeout);
                Assert.False(issuedCommands.Any(), $"There are issued commands:{Environment.NewLine}" +
                                                   issuedCommands);
                return;
            }
            if (count >= 0)
            {
                WaitForAllCommands();
                Assert.Equal(count,issuedCommands.Count(command));
                return;
            }
            while (!CommandReceived())
            {
                Assert.True(commandResetEvent.WaitOne(NamedPipeCommunicationProtocolSimulator.DefaultTimeout),
                            $"Timeout while waiting for next command. Issued commands:{Environment.NewLine}" +
                            issuedCommands);
            }

            bool CommandReceived()
            {
                return issuedCommands.Contains(command);
            }
        }

        protected void WaitForAllCommands()
        {
            while (commandResetEvent.WaitOne(NamedPipeCommunicationProtocolSimulator.DefaultTimeout))
            {
                //do nothing wait for last command
            }
        }

        public async Task SendKillCommand()
        {
            string message = "{\"type\":\"kill\"}";
            await ClientSimulator.WriteMessage(message);
        }

        public void CheckServerDisconnected()
        {
            bool result = ClientSimulator.WaitForDisconnect();
            Assert.True(result,"Server did not disconnected on kill command.");
        }

        public async Task CancelCommand(string command)
        {
            string message = string.IsNullOrEmpty(command)
                                 ? "{\"type\":\"cancel\"}"
                                 : $"{{\"type\":\"cancel\", \"command\":\"{command}\"}}";
            await ClientSimulator.WriteMessage(message);
        }

        public async Task CheckCommandCanceled(string[] command, bool canceled)
        {
            await clientSimulator.WaitForLastMessage();
            if (canceled)
            {
                Assert.True(issuedCommands.CommandCanceled(command), $"Command not found or not canceled.{Environment.NewLine}" +
                                                                     issuedCommands);
            }
            else
            {
                Assert.False(issuedCommands.CommandCanceled(command), $"Command canceled.{Environment.NewLine}" +
                                                                      issuedCommands);
            }
        }

        public void FinishCommand(string[] command, bool result)
        {
            CheckCommandTriggered(command);
            issuedCommands.FinishCommand(command, result);
        }

        public async Task CheckCommandReply(string command, bool result)
        {
            await CheckExpectedMessage($"{{\"type\":\"reply\", \"inReplyTo\":\"command\", \"command\":\"{command}\", \"parsedCommand\":{{\"error\":\"This is a test double\"}}, \"reply\":{{}}, \"success\":{result.ToString().ToLowerInvariant()}}}");
        }

        public async Task CheckNoCommandReply(string command)
        {
            string message = await ClientSimulator.ReadMessage(false);
            while (!string.IsNullOrEmpty(message))
            {
                Assert.False(IsExpectedMessage(), $"Message {message} should not have been send.");
                message = await ClientSimulator.ReadMessage(false);
            }

            bool IsExpectedMessage()
            {
                string expectedMessage1 = $"{{\"type\":\"reply\", \"inReplyTo\":\"command\", \"command\":\"{command}\", \"parsedCommand\":{{\"error\":\"This is a test double\"}}, \"reply\":{{}}, \"success\":\"true\"}}";
                JObject expectedObject1 = JObject.Parse(expectedMessage1);
                string expectedMessage2 = $"{{\"type\":\"reply\", \"inReplyTo\":\"command\", \"command\":\"{command}\", \"parsedCommand\":{{\"error\":\"This is a test double\"}}, \"reply\":{{}}, \"success\":\"false\"}}";
                JObject expectedObject2 = JObject.Parse(expectedMessage2);
                JObject actualObject = JObject.Parse(message);
                return JToken.DeepEquals(expectedObject1, actualObject) ||
                       JToken.DeepEquals(expectedObject2, actualObject);
            }
        }

        public async Task CheckCancelReply(string command)
        {
            await CheckExpectedMessage($"{{\"type\":\"reply\", \"inReplyTo\":\"cancel\", \"command\":\"{command}\", \"parsedCommand\":{{\"error\":\"This is a test double\"}}, \"reply\":{{}}, \"success\":true}}");
        }

        public async Task CheckHandshakeReply(bool result)
        {
            await CheckExpectedMessage($"{{\"type\":\"reply\", \"inReplyTo\":\"handshake\", \"reply\":{{\"supportedProtocolVersions\":" +
                                 $"[{string.Join(", ",CommunicationConstants.SupportedProtocolVersions.Select(v => $"{{\"major\":{v.Major},\"minor\":{v.Minor}}}"))}]" +
                                 $"}}, \"success\":{result.ToString().ToLowerInvariant()}}}");
        }

        public async Task CountHandshakeReplies(int count)
        {
            int actual = 0;
            string message;
            while (!string.IsNullOrEmpty(message = await ClientSimulator.ReadMessage(false)))
            {
                if (message.Contains("handshake"))
                {
                    actual++;
                }
            }

            actual.Should().Be(count);
        }

        public async Task CheckMessageReceived(string message, string messageType, string command)
        {
            await CheckExpectedMessage($"{{\"type\":\"message\", \"command\":\"{command}\", \"parsedCommand\":{{\"error\":\"This is a test double\"}}, \"message\":\"{message}\", \"messageType\":\"{messageType}\"}}");
        }

        public void CheckHeartbeatMessages(int expectedCount, bool allowMoreHeartbeats = true)
        {
            int heartbeatMessages = ClientSimulator.CountReceivedMessages(IsHeartbeat);
            if (allowMoreHeartbeats)
            {
                heartbeatMessages.Should().BeGreaterOrEqualTo(expectedCount);
            }
            else
            {
                heartbeatMessages.Should().BeLessOrEqualTo(expectedCount);
            }

            bool IsHeartbeat(string arg)
            {
                JObject expectedObject = JObject.Parse("{\"type\":\"heartbeat\"}");
                JObject actualObject = JObject.Parse(arg);
                return JToken.DeepEquals(expectedObject, actualObject);
            }
        }

        public void ReportProgress(Progress progress, string[] command)
        {
            WaitForAllCommands();
            issuedCommands.ReportProgress(progress, command);
        }

        public void StartProgress(int maximum, string[] command, string message = "", string completedMessage = "")
        {
            WaitForAllCommands();
            issuedCommands.StartProgress(maximum, command, message, completedMessage);
        }

        public void StartChildProgress(int maximum, string message)
        {
            WaitForAllCommands();
            issuedCommands.StartChildProgress(maximum, message);
        }

        public void IncrementProgress(int amount, string message)
        {
            WaitForAllCommands();
            issuedCommands.IncrementProgress(amount, message);
        }

        public void CompleteProgress(string[] command = null)
        {
            WaitForAllCommands();
            issuedCommands.CompleteProgress(command);
        }

        public void CompleteChildProgress()
        {
            WaitForAllCommands();
            issuedCommands.CompleteChildProgress();
        }
        
        public void StartInfiniteProgress(string message)
        {
            WaitForAllCommands();
            issuedCommands.StartInfiniteProgress(message);
        }

        public void StartInfiniteChildProgress(string message)
        {
            WaitForAllCommands();
            issuedCommands.StartInfiniteChildProgress(message);
        }

        public async Task CheckLastReportedProgress(int currentProgress = int.MinValue, string progressMessage = "")
        {
            string message;
            bool progressFound = false;
            await ClientSimulator.WaitForLastMessage();
            while (!string.IsNullOrEmpty((message = await ClientSimulator.ReadMessage(false))))
            {
                if (IsProgressMessage(out JObject progress))
                {
                    Assert.True(progress.ContainsKey("command"), "Command property missing");
                    Assert.True(progress.ContainsKey("parsedCommand"), "Command property missing");
                    Assert.Equal(progressMessage, progress["progressMessage"].Value<string>());
                    if (currentProgress != int.MinValue)
                    {
                        Assert.Equal(currentProgress, progress["progress"].Value<int>());
                    }
                    Assert.Equal(0, progress["progressMinimum"].Value<int>());
                    Assert.Equal(CommunicationConstants.ProgressResolution, progress["progressMaximum"].Value<int>());
                    progressFound = true;
                    break;
                }
            }

            Assert.True(progressFound,"No progress message was found");

            bool IsProgressMessage(out JObject jObject)
            {
                jObject = JObject.Parse(message);
                return jObject.ContainsKey("type") && jObject["type"].Type == JTokenType.String &&
                       jObject["type"].Value<string>().ToLowerInvariant() == "progress";
            }
        }

        private async Task CheckExpectedMessage(string expectedMessage)
        {
            JObject expectedObject = JObject.Parse(expectedMessage);
            await CheckExpectedMessage(expectedObject);
        }

        private async Task CheckExpectedMessage(JObject expectedObject, int? timeout = null)
        {
            await CheckExpectedMessage(o => JToken.DeepEquals(expectedObject, o),
                                 s =>
                                     $"Message was not expected. Continue with next message. Actual:{Environment.NewLine}" +
                                     $"{s}{Environment.NewLine}" +
                                     $"Expected:{Environment.NewLine}" +
                                     $"{expectedObject.ToString(Formatting.Indented)}",
                                 timeout);
        }

        private async Task CheckExpectedMessage(Func<JObject, bool> isExpectedMessage, Func<string, string> notExpectedLogMessage, int? timeout = null)
        {
            string message = timeout.HasValue
                                 ? await ClientSimulator.ReadMessage(timeout: timeout.Value)
                                 : await ClientSimulator.ReadMessage();
            while (!IsExpectedMessage())
            {
                Log.LogInformation(notExpectedLogMessage(message));
                message = timeout.HasValue
                              ? await ClientSimulator.ReadMessage(timeout: timeout.Value)
                              : await ClientSimulator.ReadMessage();
            }

            bool IsExpectedMessage()
            {
                JObject actualObject = JObject.Parse(message);
                return isExpectedMessage(actualObject);
            }
        }

        public async Task CheckMessageIsSameAsFile(string file, int timeout)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream($"Test.PlcNext.Deployment.TestResults.{file}").ConvertPaths())
            using (StreamReader resourceReader = new StreamReader(resourceStream))
            using (JsonReader reader = new JsonTextReader(resourceReader))
            {
                JObject expectedMessage = JObject.Load(reader, new JsonLoadSettings());
                await CheckExpectedMessage(expectedMessage, timeout);
            }
        }

        public async Task CheckNoMessage()
        {
            string message = await ClientSimulator.ReadMessage(false);
            Assert.True(string.IsNullOrEmpty(message),$"There should have been no reply, but {message} was send.");
        }

        public void RaiseCommandLineMessage(string message, string messageType)
        {
            WaitForAllCommands();
            issuedCommands.ReportCommandLineMessage(message, messageType);
        }
        
        public async Task CheckUpdateMessage(UpdateMessageType messageType, string projectName = "")
        {
            switch (messageType)
            {
                case UpdateMessageType.Setting:
                    await CheckExpectedMessage("{\"type\":\"update\", \"updateTopic\":\"settings\"}");
                    break;
                case UpdateMessageType.ProjectSettings:
                    await CheckExpectedMessage(IsExpectedProjectSettingUpdate,
                                         s =>
                                             $"Message was not expected. Continue with next message. Actual:{Environment.NewLine}" +
                                             $"{s}{Environment.NewLine}" +
                                             $"Expected:{Environment.NewLine}" +
                                             $"{{\"type\":\"update\", \"updateTopic\":\"project-settings\", \"project\":\".*{projectName}\"}}");
                    break;
                case UpdateMessageType.Sdk:
                    await CheckExpectedMessage("{\"type\":\"update\", \"updateTopic\":\"sdks\"}");
                    break;
                default:
                    Assert.True(false,$"Message type {messageType} is not yet implemented.");
                    break;
            }
            
            bool IsExpectedProjectSettingUpdate(JObject actual)
            {
                return actual.ContainsKey("type") && actual["type"].Type == JTokenType.String &&
                       actual["type"].Value<string>().ToLowerInvariant() == "update" &&
                       actual.ContainsKey("updateTopic") && actual["updateTopic"].Type == JTokenType.String &&
                       actual["updateTopic"].Value<string>() == "project-settings" &&
                       actual.ContainsKey("project") && actual["project"].Type == JTokenType.String &&
                       actual["project"].Value<string>().EndsWith(projectName);
            }
        }

        public void DisconnectClient()
        {
            ClientSimulator.Disconnect();
        }

        public void CheckSeverRegisteredDisconnect()
        {
            disconnectedResetEvent.WaitOne(NamedPipeCommunicationProtocolSimulator.DefaultTimeout).Should()
                                  .BeTrue("a disconnect was expected to register.");
        }

        private class IssuedCommands : IDisposable, IEnumerable<IssuedCommands.IssuedCommand>
        {
            private readonly List<IssuedCommand> commands = new List<IssuedCommand>();

            public void Add(string[] command, CancellationTokenSource completionSource, CancellationToken cancellationToken,
                            IUserInterface userInterface, IProgressVisualizer progressVisualizer)
            {
                commands.Add(new IssuedCommand(command, completionSource, cancellationToken, userInterface, progressVisualizer));
            }

            public void ReportCommandLineMessage(string message, string messageType, string[] command = null)
            {
                foreach (IssuedCommand issuedCommand in commands.Where(c => c.CommandEquals(command)))
                {
                    issuedCommand.ReportCommandLineMessage(message, messageType);
                }
            }

            public void ReportProgress(Progress progress, string[] command = null)
            {
                foreach (IssuedCommand issuedCommand in commands.Where(c => c.CommandEquals(command)))
                {
                    issuedCommand.ReportProgress(progress);
                }
            }

            public void StartProgress(int maximum, string[] command, string message, string completedMessage)
            {
                foreach (IssuedCommand issuedCommand in commands.Where(c => c.CommandEquals(command)))
                {
                    issuedCommand.StartProgress(maximum, message, completedMessage);
                }
            }

            public void StartChildProgress(int maximum, string message)
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.StartChildProgress(maximum, message);
                }
            }
            
            public void IncrementProgress(int amount, string message)
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.IncrementProgress(amount, message);
                }
            }

            public void CompleteProgress(string[] command)
            {
                foreach (IssuedCommand issuedCommand in commands.Where(c => c.CommandEquals(command)))
                {
                    issuedCommand.CompleteProgress();
                }
            }

            public void CompleteChildProgress()
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.CompleteChildProgress();
                }
            }
            
            public void StartInfiniteProgress(string message)
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.StartInfiniteProgress(message);
                }
            }

            public void StartInfiniteChildProgress(string message)
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.StartInfiniteChildProgress(message);
                }
            }

            public bool Contains(string[] command)
            {
                return commands.Any(c => c.CommandEquals(command));
            }

            public bool CommandCanceled(string[] command)
            {
                return commands.FirstOrDefault(c => c.CommandEquals(command))?.Canceled == true;
            }

            public int Count(string[] command)
            {
                return commands.Count(c => c.CommandEquals(command));
            }

            public void FinishCommand(string[] command, bool result)
            {
                IssuedCommand issuedCommand = commands.FirstOrDefault(c => c.CommandEquals(command));
                Assert.NotNull(issuedCommand);
                issuedCommand.Finish(result);
            }

            public int GetResult(string[] command)
            {
                IssuedCommand issuedCommand = commands.FirstOrDefault(c => c.CommandEquals(command));
                Assert.NotNull(issuedCommand);
                return issuedCommand.Result ? 0 : -1;
            }

            public IEnumerator<IssuedCommand> GetEnumerator()
            {
                return ((IEnumerable<IssuedCommand>)commands.ToArray()).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)commands.ToArray()).GetEnumerator();
            }

            public void Dispose()
            {
                foreach (IssuedCommand issuedCommand in commands)
                {
                    issuedCommand.Dispose();
                }
            }

            public override string ToString()
            {
                return string.Join(Environment.NewLine, commands);
            }

            public class IssuedCommand : IDisposable
            {
                private readonly string[] command;
                private readonly CancellationTokenSource commandCompletionSource;
                private readonly CancellationToken commandCancelToken;
                private readonly IUserInterface userInterface;
                private readonly IProgressVisualizer progressVisualizer;
                private readonly Dictionary<int, IProgressNotifier> progressNotifiers = new Dictionary<int, IProgressNotifier>();
                private readonly List<IDisposable> childProgresses = new List<IDisposable>();

                public bool Result { get; private set; }

                public IssuedCommand(string[] command, CancellationTokenSource commandCompletionSource,
                                     CancellationToken commandCancelToken, IUserInterface userInterface,
                                     IProgressVisualizer progressVisualizer)
                {
                    this.command = command;
                    this.commandCompletionSource = commandCompletionSource;
                    this.commandCancelToken = commandCancelToken;
                    this.userInterface = userInterface;
                    this.progressVisualizer = progressVisualizer;
                }

                public bool Canceled => commandCancelToken.IsCancellationRequested;

                public void Dispose()
                {
                    commandCompletionSource?.Dispose();
                }

                public override string ToString()
                {
                    return $"{nameof(command)}: {string.Join(" ",command)}, {nameof(commandCancelToken)}: 0x{commandCancelToken.GetHashCode():X}={commandCancelToken.IsCancellationRequested}";
                }

                public bool CommandEquals(string[] commandParts)
                {
                    return commandParts == null || command.SequenceEqual(commandParts);
                }

                public void Finish(bool result)
                {
                    Result = result;
                    commandCompletionSource.Cancel();
                }

                public void ReportCommandLineMessage(string message, string messageType)
                {
                    switch (messageType.ToLowerInvariant())
                    {
                        case "verbose":
                            userInterface.WriteVerbose(message);
                            break;
                        case "information":
                            userInterface.WriteInformation(message);
                            break;
                        case "warning":
                            userInterface.WriteWarning(message);
                            break;
                        case "error":
                            userInterface.WriteError(message);
                            break;
                        default:
                            Assert.True(false, $"Message type {messageType} is not known.");
                            break;
                    }
                }

                public void ReportProgress(Progress progress)
                {
                    IProgressNotifier notifier = progressNotifiers[progress.ProgressMaximum];
                    notifier.Tick(progress.CurrentProgress, progress.Message);
                }

                public void StartProgress(int maximum, string message, string completedMessage)
                {
                    IProgressNotifier notifier = progressVisualizer.Spawn(maximum,
                                                                          message,
                                                                          completedMessage);
                    progressNotifiers[maximum] = notifier;
                }

                public void StartChildProgress(int maximum, string message)
                {
                    IProgressNotifier notifier = progressNotifiers.Values.First().Spawn(maximum, message);
                    progressNotifiers[maximum] = notifier;
                    childProgresses.Add(notifier);
                }

                public void IncrementProgress(int amount, string message)
                {
                    foreach (IProgressNotifier notifier in progressNotifiers.Values)
                    {
                        notifier.TickIncrement(amount, message);
                    }
                }

                public void CompleteProgress()
                {
                    foreach (IProgressNotifier notifier in progressNotifiers.Values)
                    {
                        notifier.Dispose();
                    }
                }

                public void CompleteChildProgress()
                {
                    foreach (IDisposable childProgress in childProgresses)
                    {
                        childProgress.Dispose();
                    }
                    childProgresses.Clear();
                }

                public void StartInfiniteProgress(string message)
                {
                    progressVisualizer.SpawnInfiniteProgress(message);
                }

                public void StartInfiniteChildProgress(string message)
                {
                    childProgresses.Add(progressNotifiers.Values.First().SpawnInfiniteProgress(message));
                }
            }
        }
    }

    public enum UpdateMessageType
    {
        Setting,
        ProjectSettings,
        Sdk
    }
}
