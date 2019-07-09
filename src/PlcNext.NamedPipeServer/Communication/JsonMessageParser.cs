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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.Communication
{
    internal class JsonMessageParser : IMessageParser, IDisposable
    {
        private readonly ICommunicationProtocol communicationProtocol;
        private readonly ILog log;
        private Version usedProtocolVersion;

        public JsonMessageParser(ILog log, ICommunicationProtocol communicationProtocol)
        {
            this.log = log;
            this.communicationProtocol = communicationProtocol;
            communicationProtocol.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (StreamReader streamReader = new StreamReader(e.Message,Encoding.UTF8))
            using (JsonReader reader = new JsonTextReader(streamReader))
            {
                JObject message = JObject.Load(reader,new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                    LineInfoHandling = LineInfoHandling.Load
                });
                ParseMessage(message);
            }
        }

        private void ParseMessage(JObject message)
        {
            if (!CheckProperty("type", message, JTokenType.String))
            {
                return;
            }

            string type = message["type"].Value<string>();
            switch (type)
            {
                case "command":
                    ParseCommand();
                    break;
                case "handshake":
                    ParseHandshake();
                    break;
                case "kill":
                    ExecuteKillSequence();
                    break;
                case "cancel":
                    ParseCancel();
                    break;
                default:
                    LogError($"Message type {type} is not known. The message will be ignored.");
                    break;
            }

            void ExecuteKillSequence()
            {
                if (!CheckHandshake())
                {
                    return;
                }
                OnSuicideIssued();
            }

            void ParseCancel()
            {
                if (TryGetCommandEventArgs(out CommandEventArgs eventArgs))
                {
                    OnCommandCanceled(eventArgs);
                }
            }

            void ParseCommand()
            {
                if (TryGetCommandEventArgs(out CommandEventArgs eventArgs))
                {
                    OnCommandIssued(eventArgs);
                }
            }

            bool TryGetCommandEventArgs(out CommandEventArgs eventArgs)
            {
                eventArgs = null;
                if (!CheckHandshake())
                {
                    return false;
                }
                if (!CheckProperty("command", message, JTokenType.String))
                {
                    return false;
                }

                string command = message["command"].Value<string>();
                eventArgs = new CommandEventArgs(new Command(command));
                return true;
            }

            void ParseHandshake()
            {
                if (CheckHandshake(false))
                {
                    return;
                }
                
                if (!CheckProperty("protocolVersion", message, JTokenType.Object))
                {
                    return;
                }

                JObject protocolVersion = (JObject) message["protocolVersion"];
                if (!CheckProperty("major", protocolVersion, JTokenType.Integer))
                {
                    return;
                }
                if (!CheckProperty("minor", protocolVersion, JTokenType.Integer))
                {
                    return;
                }

                int majorVersion = protocolVersion["major"].Value<int>();
                int minorVersion = protocolVersion["minor"].Value<int>();
                usedProtocolVersion = CommunicationConstants.SupportedProtocolVersions.Where(
                                                                 v => v.Major == majorVersion &&
                                                                      v.Minor <= minorVersion)
                                                            .OrderByDescending(v => v)
                                                            .FirstOrDefault();
                OnHandshakeCompleted(new HandshakeEventArgs(usedProtocolVersion != null));
            }

            bool CheckProperty(string property, JObject currentObject, JTokenType expectedTokenType)
            {
                if (currentObject.ContainsKey(property) && currentObject[property].Type == expectedTokenType)
                {
                    return true;
                }

                LogError($"The received message does not contain the expected '{GetPropertyPath()}' property. The message will be ignored.");
                return false;

                string GetPropertyPath()
                {
                    return $"{currentObject.Path}.{property}";
                }
            }

            bool CheckHandshake(bool logResult = true)
            {
                if (usedProtocolVersion != null)
                {
                    if (logResult)
                    {
                        log.LogInformation($"Handshake is valid. Used protocol version: {usedProtocolVersion.ToString(2)}");
                    }
                    return true;
                }

                if (logResult)
                {
                    LogError("No successful handshake was communicated yet. The message will be ignored.");
                }
                return false;

            }

            void LogError(string error)
            {
                log.LogError($"{error}{Environment.NewLine}Received message:");
                using (LogWriter textWriter = new LogWriter(log))
                using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter))
                {
                    message.WriteTo(jsonWriter);
                }
            }
        }

        public event EventHandler<CommandEventArgs> CommandIssued;
        public event EventHandler<CommandEventArgs> CommandCanceled;
        public event EventHandler<EventArgs> SuicideIssued;
        public event EventHandler<HandshakeEventArgs> HandshakeCompleted;

        public void Dispose()
        {
            communicationProtocol.MessageReceived -= OnMessageReceived;
        }

        private class LogWriter : TextWriter
        {
            private readonly ILog log;
            private StringBuilder currentLine;

            public LogWriter(ILog log)
            {
                this.log = log;
            }

            public override Encoding Encoding { get; } = Encoding.UTF8;

            public override void Write(char value)
            {
                if (currentLine == null)
                {
                    currentLine = new StringBuilder();
                }

                //ignore new line characters
                if (value != '\r' && value != '\n')
                {
                    currentLine.Append(value);
                }
                else if (value == '\n')
                {
                    //Complete line
                    log.LogInformation(currentLine.ToString());
                    currentLine.Clear();
                }
            }
        }

        protected virtual void OnCommandIssued(CommandEventArgs e)
        {
            log.LogInformation($"Command message was parsed. Command: {e.Command}");
            CommandIssued?.Invoke(this, e);
        }

        protected virtual void OnSuicideIssued()
        {
            log.LogInformation($"Kill message was parsed.");
            SuicideIssued?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCommandCanceled(CommandEventArgs e)
        {
            log.LogInformation($"Command cancel message was parsed. Command: {e.Command}");
            CommandCanceled?.Invoke(this, e);
        }

        protected virtual void OnHandshakeCompleted(HandshakeEventArgs e)
        {
            HandshakeCompleted?.Invoke(this, e);
        }
    }
}
