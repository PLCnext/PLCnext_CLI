#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;
using PlcNext.CliNamedPipeMediator;
using PlcNext.NamedPipeServer.Communication;

namespace PlcNext
{
    public class BaseUpdateDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateMessagesMediator>().AsSelf().AutoActivate();
            builder.RegisterType<NamedPipeInstanceCommunicationService>().As<IInstanceCommunicationService>()
                   .InstancePerLifetimeScope().PropertiesAutowired();
            ContainerBuilder messageSenderBuilder = new ContainerBuilder();
            messageSenderBuilder.RegisterModule<MessageSenderDiModule>();
            IContainer messageSenderContainer = messageSenderBuilder.Build();
            builder.RegisterInstance(messageSenderContainer)
                   .As<IContainer>()
                   .Keyed<IContainer>(CommunicationConstants.MessageSenderContainerKey)
                   .SingleInstance();
        }
    }
}