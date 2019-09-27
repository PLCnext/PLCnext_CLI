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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools;
using PlcNext.NamedPipeServer.Data;

namespace PlcNext.NamedPipeServer.Communication
{
    internal class JsonMessageSender : IMessageSender
    {
        private readonly ICommunicationProtocol communicationProtocol;

        public JsonMessageSender(ICommunicationProtocol communicationProtocol)
        {
            this.communicationProtocol = communicationProtocol;
        }

        private Stream Convert(object message)
        {
            string serializedMessage = JsonConvert.SerializeObject(message, Formatting.Indented,
                                                                   new JsonSerializerSettings
                                                                   {
                                                                       NullValueHandling = NullValueHandling.Ignore,
                                                                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                                       StringEscapeHandling = StringEscapeHandling.EscapeHtml
                                                                   });
            return new MemoryStream(Encoding.UTF8.GetBytes(serializedMessage));
        }

        public void SendHeartbeat(Action messageCompletedAction = null)
        {
            using (Stream message = Convert(new Heartbeat()))
            {
                communicationProtocol.SendMessage(message, messageCompletedAction);
            }
        }

        public void SendCommandReply(Command command, Action messageCompletedAction = null)
        {
            using (Stream message = Convert(CommandReply.Parse(command)))
            {
                communicationProtocol.SendMessage(message, messageCompletedAction);
            }
        }

        public void SendHandshakeReply(bool success)
        {
            using (Stream message = Convert(new HandshakeReply(success,CommunicationConstants.SupportedProtocolVersions)))
            {
                communicationProtocol.SendMessage(message);
            }
        }

        public void SendMessage(string messageText, MessageType messageType, Command command)
        {
            using (Stream message = Convert(new Message(command.RawCommand,command.ParsedCommand,messageText,messageType)))
            {
                communicationProtocol.SendMessage(message);
            }
        }

        public void SendProgress(CommandProgress progress)
        {
            using (Stream progressReport = Convert(Progress.Parse(progress)))
            {
                communicationProtocol.SendMessage(progressReport);
            }
        }

        public void SendSettingsUpdated(Action messageCompletedAction = null)
        {
            using (Stream updateMessage = Convert(new SettingsUpdate()))
            {
                communicationProtocol.SendMessage(updateMessage, messageCompletedAction);
            }
        }

        public void SendProjectSettingsUpdate(string path,Action messageCompletedAction = null)
        {
            using (Stream updateMessage = Convert(new ProjectSettingsUpdate(path)))
            {
                communicationProtocol.SendMessage(updateMessage, messageCompletedAction);
            }
        }

        public void SendSdksUpdated(Action messageCompletedAction = null)
        {
            using (Stream updateMessage = Convert(new SdksUpdate()))
            {
                communicationProtocol.SendMessage(updateMessage, messageCompletedAction);
            }
        }

        private class SdksUpdate : Update
        {
            public SdksUpdate() : base("sdks")
            {
            }
        }

        private class ProjectSettingsUpdate : Update
        {
            public ProjectSettingsUpdate(string path) : base("project-settings")
            {
                Path = path;
            }

            [JsonProperty(PropertyName = "project")]
            public string Path { get; }
        }

        private class SettingsUpdate : Update
        {
            public SettingsUpdate() : base("settings")
            {
            }
        }

        private class Update : BaseMessage
        {
            public Update(string updateTopic) : base("update")
            {
                UpdateTopic = updateTopic;
            }

            [JsonProperty(PropertyName = "updateTopic")]
            public string UpdateTopic { get; }
        } 

        private class Progress : BaseMessage
        {
            private Progress(string rawCommand, string parsedCommand, int progressValue, int progressMinimum, int progressMaximum, string progressMessage, Guid progressId) : base("progress")
            {
                ProgressValue = progressValue;
                ProgressMinimum = progressMinimum;
                ProgressMaximum = progressMaximum;
                ProgressMessage = progressMessage;
                ProgressId = progressId;
                RawCommand = rawCommand;
                try
                {
                    ParsedCommand = JObject.Parse(parsedCommand);
                }
                catch (JsonReaderException)
                {
                    //no need to log anything
                    ParsedCommand = parsedCommand;
                }
            }

            public static Progress Parse(CommandProgress commandProgress)
            {
                return new Progress(commandProgress.Command.RawCommand, commandProgress.Command.ParsedCommand,
                                    (int) Math.Round(commandProgress.NormalizedProgress *
                                                     CommunicationConstants.ProgressResolution),
                                    0, CommunicationConstants.ProgressResolution,
                                    commandProgress.Message??string.Empty, commandProgress.ProgressId);
            }

            [JsonProperty(PropertyName = "command")]
            public string RawCommand { get; }
            
            [JsonProperty(PropertyName = "parsedCommand")]
            public object ParsedCommand { get; }
            
            [JsonProperty(PropertyName = "progress")]
            public int ProgressValue { get; }
            
            [JsonProperty(PropertyName = "progressMinimum")]
            public int ProgressMinimum { get; }
            
            [JsonProperty(PropertyName = "progressMaximum")]
            public int ProgressMaximum { get; }
            
            [JsonProperty(PropertyName = "progressMessage")]
            public string ProgressMessage { get; }
            
            [JsonProperty(PropertyName = "progressId", ItemConverterType = typeof(GuidConverter))]
            public Guid ProgressId { get; }

            private class GuidConverter : JsonConverter<Guid>
            {
                public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
                {
                    writer.WriteValue(value.ToByteString());
                }

                public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
                {
                    throw new InvalidOperationException("Cannot convert back");
                }
            }
        }

        private class Message : BaseMessage
        {
            public Message(string rawCommand, string parsedCommand, string messageContent, MessageType messageType) : base("message")
            {
                MessageContent = messageContent;
                MessageType = messageType.ToString("G").ToLowerInvariant();
                RawCommand = rawCommand;
                try
                {
                    ParsedCommand = JObject.Parse(parsedCommand);
                }
                catch (JsonReaderException)
                {
                    //no need to log anything
                    ParsedCommand = parsedCommand;
                }
            }

            [JsonProperty(PropertyName = "command")]
            public string RawCommand { get; }
            
            [JsonProperty(PropertyName = "parsedCommand")]
            public object ParsedCommand { get; }
            
            [JsonProperty(PropertyName = "message")]
            public string MessageContent { get; }
            
            [JsonProperty(PropertyName = "messageType")]
            public string MessageType { get; }
        }

        private class CommandReply : Reply
        {
            private CommandReply(string rawCommand, string parsedCommand, string inReplyTo, bool success, object replyValue = null) : base(inReplyTo, success, replyValue)
            {
                RawCommand = rawCommand;
                try
                {
                    ParsedCommand = JObject.Parse(parsedCommand);
                }
                catch (JsonReaderException)
                {
                    //no need to log anything
                    ParsedCommand = parsedCommand;
                }
            }

            public static CommandReply Parse(Command command)
            {
                return command.IsCanceled ? new CommandReply(command.RawCommand,command.ParsedCommand,"cancel",true) : new CommandReply(command.RawCommand,command.ParsedCommand,"command",command.Result,command.DetailedResult);
            }

            [JsonProperty(PropertyName = "command")]
            public string RawCommand { get; }
            
            [JsonProperty(PropertyName = "parsedCommand")]
            public object ParsedCommand { get; }
        }
        
        private class HandshakeReply : Reply
        {
            public HandshakeReply(bool success, Version[] supportedVersions) 
                : base("handshake", success, new HandshakeReplyContent(supportedVersions))
            {

            }

            private class HandshakeReplyContent
            {
                public HandshakeReplyContent(Version[] supportedVersions)
                {
                    SupportedVersions = supportedVersions;
                }

                [JsonProperty(PropertyName = "supportedProtocolVersions", ItemConverterType = typeof(VersionConverter))]
                public Version[] SupportedVersions { get; }
            }

            private class VersionConverter : JsonConverter<Version>
            {
                public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("major");
                    writer.WriteValue(value.Major);
                    writer.WritePropertyName("minor");
                    writer.WriteValue(value.Minor);
                    writer.WriteEndObject();
                }

                public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue,
                                                 JsonSerializer serializer)
                {
                    throw new InvalidOperationException("Cannot read back value.");
                }
            }
        }

        private class Reply : BaseMessage
        {
            public Reply(string inReplyTo, bool success, object replyValue = null) : base("reply")
            {
                InReplyTo = inReplyTo;
                ReplyValue = replyValue ?? new object();
                Success = success;
            }

            [JsonProperty(PropertyName = "inReplyTo")]
            public string InReplyTo { get; }
            
            [JsonProperty(PropertyName = "reply")]
            public object ReplyValue { get; }
            
            [JsonProperty(PropertyName = "success")]
            public bool Success { get; }
        }

        private class Heartbeat : BaseMessage
        {
            public Heartbeat() : base("heartbeat")
            {
                
            }
        }

        private class BaseMessage
        {
            public BaseMessage(string type)
            {
                Type = type;
            }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; }
        }
    }
}
