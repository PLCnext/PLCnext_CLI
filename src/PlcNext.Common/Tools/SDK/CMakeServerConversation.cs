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
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.UI;
using static PlcNext.Common.Tools.Constants;

namespace PlcNext.Common.Tools.SDK
{
    public class CMakeCodeModelReplyMessage : CMakeReplyMessage
    {
        public CMakeCodeModelReplyMessage(JObject content, JArray codeModel) : base(content)
        {
            CodeModel = codeModel;
        }

        public JArray CodeModel { get; }

        public new static CMakeCodeModelReplyMessage Create(JObject content)
        {
            if (!(content["configurations"] is JArray codeModel))
            {
                throw new FormattableException("Expected reply message to contain configurations array.");
            }

            return new CMakeCodeModelReplyMessage(content, codeModel);
        }
    }

    public class CMakeCacheReplyMessage : CMakeReplyMessage
    {
        public CMakeCacheReplyMessage(JObject content, JArray cache) : base(content)
        {
            Cache = cache;
        }

        public JArray Cache { get; }

        public new static CMakeCacheReplyMessage Create(JObject content)
        {
            if (!(content["cache"] is JArray cache))
            {
                throw new FormattableException("Expected reply message to contain cache array.");
            }

            return new CMakeCacheReplyMessage(content, cache);
        }
    }

    public class CMakeReplyMessage : CMakeBaseResponseMessage
    {
        protected CMakeReplyMessage(JObject content) : base(content)
        {
        }

        public static CMakeReplyMessage Create(JObject content)
        {
            switch (content["inReplyTo"].Value<string>())
            {
                case "codemodel":
                    return CMakeCodeModelReplyMessage.Create(content);
                case "cache":
                    return CMakeCacheReplyMessage.Create(content);
                default:
                    return new CMakeReplyMessage(content);
            }
        }
    }

    public class CMakeProgressMessage : CMakeMessage
    {
        private CMakeProgressMessage()
        {

        }

        public static CMakeProgressMessage Create(JObject content)
        {
            return new CMakeProgressMessage();
        }
    }

    public class CMakeMessageMessage : CMakeBaseResponseMessage
    {
        public string Title { get; }
        public string Message { get; }

        private CMakeMessageMessage(string message, string title, JObject content) : base(content)
        {
            Message = message;
            Title = title;
        }

        public static CMakeMessageMessage Create(JObject content)
        {
            string title = content.ContainsKey("title")
                               ? content["title"].Value<string>()
                               : string.Empty;
            string message = content["message"].Value<string>();

            return new CMakeMessageMessage(message, title, content);
        }
    }

    public class CMakeBaseResponseMessage : CMakeMessage
    {
        private readonly JObject content;

        public CMakeBaseResponseMessage(JObject content)
        {
            this.content = content;
            Cookie = content["cookie"].Value<string>();
            ResponseType = content["inReplyTo"].Value<string>();
        }

        public string Cookie { get; }
        public string ResponseType { get; }

        public override string ToString()
        {
            return content.ToString(Formatting.Indented);
        }
    }

    public class CMakeHelloMessage : CMakeMessage
    {
        private CMakeHelloMessage(IEnumerable<Version> supportedProtocolVersions)
        {
            SupportedProtocolVersions = supportedProtocolVersions;
        }

        public static CMakeHelloMessage Create(JObject content)
        {
            JArray array = content["supportedProtocolVersions"] as JArray;
            if (array == null)
            {
                throw new FormattableException("Expected hello mesage to contain supportedProtocolVersions array");
            }
            List<Version> supportedVersions = new List<Version>();
            foreach (JToken token in array)
            {
                int major = token["major"].Value<int>();
                int minor = token["minor"].Value<int>();
                supportedVersions.Add(new Version(major, minor));
            }
            return new CMakeHelloMessage(supportedVersions);
        }

        public IEnumerable<Version> SupportedProtocolVersions { get; }
    }

    public abstract class CMakeMessage
    {
        public static T Parse<T>(string message, ILog log) where T : CMakeMessage
        {
            try
            {
                CMakeMessage cMakeMessage;
                JObject content = JObject.Parse(message, new JsonLoadSettings { LineInfoHandling = LineInfoHandling.Ignore });
                log.LogVerbose($"Recieved message from server:{Environment.NewLine}" +
                               $"{content.ToString(Formatting.Indented)}");
                switch (content["type"].Value<string>())
                {
                    case "hello":
                        cMakeMessage = CMakeHelloMessage.Create(content);
                        break;
                    case "reply":
                        cMakeMessage = CMakeReplyMessage.Create(content);
                        break;
                    case "progress":
                        cMakeMessage = CMakeProgressMessage.Create(content);
                        break;
                    case "message":
                        cMakeMessage = CMakeMessageMessage.Create(content);
                        break;
                    default:
                        throw new FormattableException($"Unkown message type {content["type"].Value<string>()}.{Environment.NewLine}" +
                                                            $"Complete message: {message}");
                }

                if (cMakeMessage is T converted)
                {
                    return converted;
                }
                throw new FormattableException($"Excpeted message of type {typeof(T)}, but message was of type  {cMakeMessage.GetType()}");
            }
            catch (JsonReaderException e)
            {
                throw new FormattableException("Error while parsing the response json.", e);
            }
        }
    }

    internal class CMakeConversation : IDisposable
    {
        private readonly IProcess serverProcess;
        private readonly NamedPipeClientStream clientStream;
        private readonly CMakeServerStream serverStream;
        private readonly ILog log;
        private string cookie;

        private CMakeConversation(IProcess serverProcess, NamedPipeClientStream clientStream, CMakeServerStream serverStream, ILog log)
        {
            this.serverProcess = serverProcess;
            this.clientStream = clientStream;
            this.serverStream = serverStream;
            this.log = log;
        }

        private async Task Handshake(string sourceDirectory, string buildDirectory,
                                    string makefileGenerator = "Unix Makefiles")
        {
            string message = $"{{\"cookie\":\"HypnoticCookieCutter\",\"type\":\"handshake\",\"protocolVersion\":{{\"major\":1}}, " +
                             $"\"sourceDirectory\":\"{sourceDirectory}\",\"buildDirectory\":\"{buildDirectory}\", " +
                             $"\"generator\":\"{makefileGenerator}\"}}";
            await serverStream.WriteMessage(message).TimeoutAfter(CMakeServerTimeout);
            CMakeReplyMessage reply = await WaitForReply("handshake");
            cookie = reply.Cookie;
        }

        private async Task Configure()
        {
            string message = $"{{\"cookie\":\"{cookie}\",\"type\":\"configure\"}}";
            await serverStream.WriteMessage(message).TimeoutAfter(CMakeServerTimeout);
            await WaitForReply("configure");

            message = $"{{\"cookie\":\"{cookie}\",\"type\":\"compute\"}}";
            await serverStream.WriteMessage(message).TimeoutAfter(CMakeServerTimeout);
            await WaitForReply("compute");
            //Replies of contigure and compute have no useful informations
        }

        public async Task<JArray> GetCodeModel()
        {
            string message = $"{{\"cookie\":\"{cookie}\",\"type\":\"codemodel\"}}";
            await serverStream.WriteMessage(message).TimeoutAfter(CMakeServerTimeout);
            CMakeCodeModelReplyMessage reply = (CMakeCodeModelReplyMessage)await WaitForReply("codemodel");
            return reply.CodeModel;
        }

        public async Task<JArray> GetCache()
        {
            string message = $"{{\"cookie\":\"{cookie}\",\"type\":\"cache\"}}";
            await serverStream.WriteMessage(message).TimeoutAfter(CMakeServerTimeout);
            CMakeCacheReplyMessage reply = (CMakeCacheReplyMessage)await WaitForReply("cache");
            return reply.Cache;
        }

        private async Task<CMakeReplyMessage> WaitForReply(string type)
        {
            CMakeProgressMessage progressMessage;
            CMakeMessageMessage messageMessage;
            CMakeMessage message;
            do
            {
                //Expect at least a progress message after default timeout
                message = CMakeMessage.Parse<CMakeMessage>(await serverStream.ReadMessage().TimeoutAfter(CMakeServerTimeout), log);
                progressMessage = message as CMakeProgressMessage;
                messageMessage = message as CMakeMessageMessage;
                if (messageMessage != null)
                {
                    CheckCookieAndType(messageMessage);
                    if (messageMessage.Title.ToLowerInvariant().Contains("error"))
                    {
                        throw new FormattableException($"CMake discovered an error.{Environment.NewLine}" +
                                                            $"{messageMessage.Title}{Environment.NewLine}" +
                                                            $"==================================={Environment.NewLine}" +
                                                            $"{messageMessage.Message}");
                    }
                }
            } while (progressMessage != null ||
                     messageMessage != null);

            CMakeReplyMessage replyMessage = (CMakeReplyMessage)message;
            CheckCookieAndType(replyMessage);
            return replyMessage;

            void CheckCookieAndType(CMakeBaseResponseMessage responseMessage)
            {
                if (!string.IsNullOrEmpty(cookie) &&
                    responseMessage.Cookie != cookie)
                {
                    throw new FormattableException($"Expected a message with the cookie {cookie}, " +
                                                        $"but got a message with the cookie {responseMessage.Cookie}. " +
                                                        "Someone else is taking to the server unexpectedly.");
                }

                if (responseMessage.ResponseType != type)
                {
                    throw new FormattableException($"Expected a message in response to {type}, " +
                                                        $"but got a message in response to {responseMessage.ResponseType}. " +
                                                        "Someone else is taking to the server unexpectedly " +
                                                        "or the server is confusing.");
                }
            }
        }

        public static async Task<CMakeConversation> Start(IProcessManager processManager,
                                                          IBinariesLocator binariesLocator,
                                                          VirtualDirectory tempDirectory, bool isWindowsSystem,
                                                          ExecutionContext executionContext,
                                                          VirtualDirectory sourceDirectory,
                                                          VirtualDirectory binaryDirectory)
        {
            IProcess process = null;
            NamedPipeClientStream pipeClient = null;
            try
            {
                string pipeName = isWindowsSystem
                                      ? $"{tempDirectory.FullName}\\.cmakeserver"
                                      : $"/tmp/cmake-server-{Guid.NewGuid().ToByteString()}";
                string serverCommand = isWindowsSystem
                                           ? $"-E server --experimental --pipe=\"\\\\?\\pipe\\{pipeName}\""
                                           : $"-E server --experimental --pipe={pipeName}";
                process = processManager.StartProcess(binariesLocator.GetExecutableCommand("cmake"), serverCommand, executionContext, showOutput: false,
                                                      showError: false, killOnDispose: true);
                pipeClient = new NamedPipeClientStream(".", pipeName,
                                                       PipeDirection.InOut, PipeOptions.Asynchronous,
                                                       TokenImpersonationLevel.Impersonation);
                pipeClient.Connect(CMakeServerTimeout);
                if (!pipeClient.IsConnected)
                {
                    throw new FormattableException("Could not connect to server");
                }

                CMakeServerStream serverStream = new CMakeServerStream(pipeClient, executionContext);
                CMakeHelloMessage hello = CMakeMessage.Parse<CMakeHelloMessage>(await serverStream.ReadMessage().TimeoutAfter(CMakeServerTimeout), executionContext);

                if (hello.SupportedProtocolVersions.All(v => v.Major != 1))
                {
                    throw new FormattableException("CMake server does not support the protocol version 1.X. " +
                                                        $"Supported versions are {string.Join(", ", hello.SupportedProtocolVersions)}");
                }

                CMakeConversation conversation = new CMakeConversation(process, pipeClient, serverStream, executionContext);

                await conversation.Handshake(sourceDirectory.FullName.Replace('\\', '/'),
                                             binaryDirectory.FullName.Replace('\\', '/'));
                await conversation.Configure();

                return conversation;
            }
            catch (Exception)
            {
                pipeClient?.Dispose();
                process?.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            serverProcess?.Dispose();
            clientStream?.Dispose();
        }
    }

    public class CMakeServerStream
    {
        private readonly ILog log;
        private NamedPipeClientStream ioStream;
        private UTF8Encoding streamEncoding;
        private const string StartTag = "\n[== \"CMake Server\" ==[\n";
        private const string EndTag = "\n]== \"CMake Server\" ==]\n";
        private const int BufferSize = 1024;
        private static readonly Regex MessageDecoder = new Regex(@"\[== ""CMake Server"" ==\[\s*(?<message>.*?)\s*\]== ""CMake Server"" ==\]", RegexOptions.Compiled);

        public CMakeServerStream(NamedPipeClientStream ioStream, ILog log)
        {
            this.ioStream = ioStream;
            this.log = log;
            streamEncoding = new UTF8Encoding();
        }

        public async Task<string> ReadMessage()
        {
            StringBuilder message = new StringBuilder();
            int readBytes = 0;
            do
            {
                byte[] buffer = new byte[BufferSize];
                readBytes = await ioStream.ReadAsync(buffer, 0, BufferSize);
                message.Append(streamEncoding.GetString(buffer, 0, readBytes));

            } while (readBytes == BufferSize && !EndsWithEndTag());

            return Decode(message.ToString());

            bool EndsWithEndTag()
            {
                int index = message.Length - 1;
                foreach (char c in EndTag.Reverse())
                {
                    if (c != message[index])
                    {
                        return false;
                    }

                    index--;
                    if (index < 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            string Decode(string completeMessage)
            {
                Match decoderMatch = MessageDecoder.Match(completeMessage);
                return decoderMatch.Success ? decoderMatch.Groups["message"].Value : completeMessage;
            }
        }

        public async Task WriteMessage(string message)
        {
            log.LogVerbose($"Post message to cmake server:{Environment.NewLine}" +
                           $"{message}");
            string completeMessage = StartTag + message + EndTag;
            byte[] outBuffer = streamEncoding.GetBytes(completeMessage);
            int len = outBuffer.Length;
            if (len > ushort.MaxValue)
            {
                len = ushort.MaxValue;
            }
            await ioStream.WriteAsync(outBuffer, 0, len);
            ioStream.Flush();
        }
    }

    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
            {
                return await task;
            }

            throw new TimeoutException();
        }
        public static async Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
            {
                await task;
            }
            else
            {
                throw new TimeoutException();
            }
        }
    }
}
