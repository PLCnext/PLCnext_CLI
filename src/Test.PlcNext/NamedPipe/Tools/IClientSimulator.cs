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
using System.Threading.Tasks;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;

namespace Test.PlcNext.NamedPipe.Tools
{
    public interface IClientSimulator
    {
        Task WriteMessage(string message, byte[] header = null, int count = 1);
        Task WriteMessage(Stream data, byte[] header = null, int count = 1);
        Task WaitForLastMessage(int timeout = NamedPipeCommunicationProtocolSimulator.DefaultTimeout);
        Task<string> ReadMessage(bool checkTimeout = true,
                                 int timeout = NamedPipeCommunicationProtocolSimulator.DefaultTimeout);
        void Dispose();
        void UseErrorConfirmation(int count);
        Task<bool> WaitForMessages(int messageCount, Func<string, bool> messageFilter = null);
        int CountReceivedMessages(Func<string, bool> messageFilter = null);
        bool WaitForDisconnect();
        void SkipConfirmation(int count);
        void Disconnect();
        Task<bool> LastMessageWasSplit();
        Task<byte> GetLastConfirmationFlag();
    }
}