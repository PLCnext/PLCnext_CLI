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
using PlcNext.Common.Tools.IO;
using PlcNext.NamedPipeServer;
using PlcNext.NamedPipeServer.Communication;

namespace Test.PlcNext.NamedPipe.Tools
{
    public static class RandomTools
    {
        public static Stream GenerateRandomStream(this StreamFactory streamFactory, long length)
        {
            Random random = new Random();
            long intervals = (length) / NamedPipeCommunicationProtocol.BufferSize;
            long remaining = (length) % NamedPipeCommunicationProtocol.BufferSize;
            Stream dataStream = streamFactory.Create(length);
            for (long i = 0; i < intervals; i++)
            {
                byte[] buffer = new byte[NamedPipeCommunicationProtocol.BufferSize];
                random.NextBytes(buffer);
                dataStream.Write(buffer, 0, buffer.Length);
            }

            byte[] remainingBuffer = new byte[remaining];
            random.NextBytes(remainingBuffer);
            dataStream.Write(remainingBuffer, 0, remainingBuffer.Length);
            dataStream.Seek(0, SeekOrigin.Begin);
            return dataStream;
        }
    }
}
