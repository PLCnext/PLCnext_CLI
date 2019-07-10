#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
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

        public static void Reset(this AsyncAutoResetEvent resetEvent)
        {
            try
            {
                resetEvent.Wait(new CancellationToken(true));
            }
            catch (Exception)
            {
                //do nothing only canceled exceptions
            }
        }

        public static async Task<bool> WaitOneWithAliveEvent(this AsyncAutoResetEvent waitHandle, int timeout,
                                                 AsyncAutoResetEvent aliveEvent)
        {
            if (await waitHandle.WaitOne(timeout))
            {
                return true;
            }
            while (await aliveEvent.WaitOne(timeout))
            {
                if (await waitHandle.WaitOne(0))
                {
                    return true;
                }
            }
            if (await waitHandle.WaitOne(timeout))
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> WaitOne(this AsyncAutoResetEvent resetEvent, int timeout)
        {
            using (CancellationTokenSource timeoutSource = new CancellationTokenSource())
            {
                Task waitTask = resetEvent.WaitAsync(timeoutSource.Token);
                timeoutSource.CancelAfter(timeout);
                try
                {
                    await waitTask;
                }
                catch (Exception)
                {
                    //do nothing only canceled exceptions
                }
                return !waitTask.IsCanceled;
            }
        }
    }
}
