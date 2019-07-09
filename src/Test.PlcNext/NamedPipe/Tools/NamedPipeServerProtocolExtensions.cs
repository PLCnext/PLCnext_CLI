#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Tools.IO;
using PlcNext.NamedPipeServer;
using PlcNext.NamedPipeServer.Communication;

namespace Test.PlcNext.NamedPipe.Tools
{
    internal static class NamedPipeServerProtocolExtensions
    {
        public static void SendMessage(this ICommunicationProtocol communicationProtocol, string message)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                communicationProtocol.SendMessage(memoryStream);
            }
        }

        public static string ToUtf8String(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.ReadToEnd());
        }

        public static bool WaitOneWithAliveEvent(this EventWaitHandle waitHandle, int timeout,
                                                 AutoResetEvent aliveEvent)
        {
            if (waitHandle.WaitOne(timeout))
            {
                return true;
            }
            while (aliveEvent.WaitOne(timeout))
            {
                if (waitHandle.WaitOne(0))
                {
                    return true;
                }
            }
            if (waitHandle.WaitOne(timeout))
            {
                return true;
            }

            return false;
        }
    }
}
