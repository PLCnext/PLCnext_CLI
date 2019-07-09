#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.AttributeFilters;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.UI;
using PlcNext.NamedPipeServer.Tools;

namespace PlcNext.NamedPipeServer.Communication
{
    public class NamedPipeInstanceCommunicationService : IInstanceCommunicationService
    {
        private readonly IContainer messageSenderContainer;
        private readonly ILog log;
        private readonly StreamFactory streamFactory;
        private readonly IEnvironmentInformation environmentInformation;
        private readonly CancellationToken cancellationToken;

        public NamedPipeInstanceCommunicationService([KeyFilter(CommunicationConstants.MessageSenderContainerKey)]IContainer messageSenderContainer, 
                                                     ILog log, StreamFactory streamFactory, IEnvironmentInformation environmentInformation, CancellationToken cancellationToken)
        {
            this.messageSenderContainer = messageSenderContainer;
            this.log = log;
            this.streamFactory = streamFactory;
            this.environmentInformation = environmentInformation;
            this.cancellationToken = cancellationToken;
        }

        public async Task<ITemporaryCommunicationChannel> OpenCommunicationChannel(int otherInstanceProcessId)
        {
            return await NamedPipeInstanceCommunicationChannel.OpenChannel(
                       environmentInformation.InterProcessServerNameBase +
                       $"{otherInstanceProcessId:D}", 
                       streamFactory, messageSenderContainer,
                       log, cancellationToken);
        }
    }
}