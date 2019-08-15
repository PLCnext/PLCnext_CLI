#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.CommandLine
{
    internal class CommandLineFacade : ICommandLineFacade
    {
        private readonly ICommandLineBuilder commandLineBuilder;
        private readonly ConcurrentDictionary<string, CancellationTokenSource> executingCommands =
            new ConcurrentDictionary<string, CancellationTokenSource>();
        private readonly ILog log;

        public CommandLineFacade(ICommandLineBuilder commandLineBuilder, ILog log)
        {
            this.commandLineBuilder = commandLineBuilder;
            this.log = log;
        }

        public Task<Command> ExecuteCommand(Command command)
        {
            if (executingCommands.ContainsKey(command.RawCommand))
            {
                log.LogError($"The command '{command}' is already executing. The second trigger will be ignored.");
                return null;
            }
            CancellationTokenSource source = new CancellationTokenSource();
            executingCommands.TryAdd(command.RawCommand, source);
            InteractiveCommand interactiveCommand = new InteractiveCommand(this);
            IDisposableCommandLineParser commandLineParser = commandLineBuilder.BuildCommandLineInstance(interactiveCommand,
                                                                                                         interactiveCommand,
                                                                                                         interactiveCommand,
                                                                                                         source.Token);
            
            string[] commandParts = command.RawCommand.SplitCommandLine().ToArray();
            command = command.WithParsedCommand(commandLineParser.GetParseResult(commandParts));
            interactiveCommand.SetCommand(command);

            return commandLineParser.Parse(commandParts).ContinueWith(task =>
            {
                try
                {
                    int result;
                    if (task.Exception != null)
                    {
                        log.LogError($"Exception while executing command '{command.RawCommand}'.{Environment.NewLine}" +
                                     $"{task.Exception}");
                        result = -1;
                    }
                    else
                    {
                        result = task.Result;
                    }
                
                    executingCommands.TryRemove(command.RawCommand, out _);
                    if (source.IsCancellationRequested)
                    {
                        command = command.AsCanceled();
                    }

                    if (interactiveCommand.CommandResult != null)
                    {
                        command = command.WithDetailedResult(interactiveCommand.CommandResult);
                    }
                    return command.WithResult(result == 0);
                }
                finally
                {
                    commandLineParser.Dispose();
                }
            }, CancellationToken.None);
        }

        public void CancelCommand(Command command)
        {
            if (executingCommands.TryGetValue(command.RawCommand, out CancellationTokenSource tokenSource))
            {
                tokenSource.Cancel();
                log.LogInformation($"Command '{command.RawCommand}' was canceled with token 0x{tokenSource.Token.GetHashCode():X}.");
            }
            else
            {
                log.LogError($"Could not cancel command '{command.RawCommand}' as it was not received. Received commands are:{Environment.NewLine}" +
                             $"{string.Join(Environment.NewLine,executingCommands.Keys)}");
            }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ProgressReceivedEventArgs> ProgressReceived;
        public event EventHandler<InfiniteProgressStartedEventArgs> InfiniteProgressStarted;
        public void CancelAllCommands()
        {
            foreach (KeyValuePair<string,CancellationTokenSource> executingCommand in executingCommands)
            {
                executingCommand.Value.Cancel();
                log.LogInformation($"Command '{executingCommand.Key}' was canceled with token 0x{executingCommand.Value.Token.GetHashCode():X}.");
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnProgressReceived(ProgressReceivedEventArgs e)
        {
            ProgressReceived?.Invoke(this, e);
        }

        protected virtual void OnInfiniteProgressStarted(InfiniteProgressStartedEventArgs e)
        {
            InfiniteProgressStarted?.Invoke(this, e);
        }

        private class InteractiveCommand : IUserInterface, IProgressVisualizer, ICommandResultVisualizer
        {
            private readonly CommandLineFacade facade;
            private Command command;
            private readonly Stack<(string, MessageType)> initialSendMessages = new Stack<(string, MessageType)>();
            private readonly object syncRoot = new object();

            public InteractiveCommand(CommandLineFacade facade)
            {
                this.facade = facade;
            }
            
            public JObject CommandResult { get; private set; }

            public void SetCommand(Command command)
            {
                lock (syncRoot)
                {
                    this.command = command;
                    foreach ((string message, MessageType type) in initialSendMessages)
                    {
                        facade.OnMessageReceived(new MessageReceivedEventArgs(command, message, type));
                    }
                    initialSendMessages.Clear();
                }
            }

            private void Send(string message, MessageType type)
            {
                lock (syncRoot)
                {
                    if (command == null)
                    {
                        initialSendMessages.Push((message, type));
                    }
                    else
                    {
                        facade.OnMessageReceived(new MessageReceivedEventArgs(command, message, type));
                    }
                }
            }

            public void WriteInformation(string message)
            {
                Send(message, MessageType.Information);
            }

            public void WriteVerbose(string message)
            {
                Send(message, MessageType.Verbose);
            }

            public void WriteError(string message)
            {
                Send(message, MessageType.Error);
            }

            public void SetVerbosity(bool verbose)
            {
                //not necessary
            }

            public void WriteWarning(string message)
            {
                Send(message, MessageType.Warning);
            }

            public void PauseOutput()
            {
                //not necessary
            }

            public void ResumeOutput()
            {
                //not necessary
            }

            public void SetQuiet(bool quiet)
            {
                //not necessary
            }

            public IProgressNotifier Spawn(double maxTicks, string startMessage, string completedMessage)
            {
                return new ProgressNotifierReporter(completedMessage, facade, maxTicks, startMessage, command);
            }

            public IDisposable SpawnInfiniteProgress(string startMessage)
            {
                facade.OnInfiniteProgressStarted(new InfiniteProgressStartedEventArgs(startMessage, command));
                return new NopDisposable();
            }

            public void Visualize(object result)
            {
                CommandResult = JObject.FromObject(result, JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    StringEscapeHandling = StringEscapeHandling.EscapeHtml
                }));
            }

            private class ProgressNotifierReporter : IProgressNotifier
            {
                private readonly Guid progressId = Guid.NewGuid();
                private readonly string completedMessage;
                private readonly CommandLineFacade facade;
                private readonly double maxTicks;
                private readonly Command command;
                private readonly List<ProgressNotifierReporter> children = new List<ProgressNotifierReporter>();
                private double progress = 0;

                public ProgressNotifierReporter(string completedMessage, CommandLineFacade facade, double maxTicks,
                                                string startMessage, Command command)
                {
                    this.completedMessage = completedMessage;
                    this.facade = facade;
                    this.maxTicks = maxTicks;
                    this.command = command;
                    ReportProgress(startMessage);
                }

                private readonly ProgressNotifierReporter parent;

                public ProgressNotifierReporter(ProgressNotifierReporter parent, double maxTicks, string startMessage)
                {
                    this.parent = parent;
                    this.maxTicks = maxTicks;
                    ReportProgress(startMessage);
                }

                public void Dispose()
                {
                    foreach (ProgressNotifierReporter child in children.ToArray())
                    {
                        child.Dispose();
                    }
                    progress = maxTicks;
                    ReportProgress(completedMessage);
                    OnDisposed();
                }

                public event EventHandler<EventArgs> Disposed; 

                protected virtual void OnDisposed()
                {
                    Disposed?.Invoke(this, EventArgs.Empty);
                }

                public void TickIncrement(double addedProgress = 1, string message = "")
                {
                    progress += addedProgress;
                    ReportProgress(message);
                }

                public void Tick(double totalProgress, string message = "")
                {
                    progress = totalProgress;
                    ReportProgress(message);
                }

                private void ReportProgress(string message)
                {
                    if (parent != null)
                    {
                        parent.ReportProgress(message);
                        return;
                    }
                    facade.OnProgressReceived(new ProgressReceivedEventArgs(new CommandProgress(command,
                                                                                                message,
                                                                                                progressId,
                                                                                                NormalizedProgress)));
                }

                private double NormalizedProgress
                {
                    get
                    {
                        double childrenProgress = CalculateChildrenProgress();
                        return progress / maxTicks + childrenProgress;

                        double CalculateChildrenProgress()
                        {
                            double increment = 1 / maxTicks;
                            return children.Aggregate(0d, (i, child) => i + child.NormalizedProgress * increment);
                        }
                    }
                }

                public IProgressNotifier Spawn(double maxTicks, string startMessage = "")
                {
                    ProgressNotifierReporter child = new ProgressNotifierReporter(this, maxTicks, startMessage);
                    children.Add(child);
                    child.Disposed += ChildOnDisposed;
                    return child;
                    
                    void ChildOnDisposed(object sender, EventArgs e)
                    {
                        child.Disposed -= ChildOnDisposed;
                        children.Remove(child);
                    }
                }

                public IDisposable SpawnInfiniteProgress(string startMessage)
                {
                    ReportProgress(startMessage);
                    return new DisposeAction(() =>
                    {
                        progress++;
                        ReportProgress(string.Empty);
                    });
                }
            }

            private class NopDisposable : IDisposable
            {
                public void Dispose()
                {
                }
            }
        }
    }
}
