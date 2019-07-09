#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;

namespace PlcNext.NamedPipeServer.Communication
{
    public class MessageSenderDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonMessageSender>().As<IMessageSender>().InstancePerLifetimeScope();
        }
    }
}