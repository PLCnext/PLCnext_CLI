#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;

namespace PlcNext.NamedPipeServer.Communication
{
    public interface IInstanceCommunicationService
    {
        Task<ITemporaryCommunicationChannel> OpenCommunicationChannel(int otherInstanceProcessId);
    }
}