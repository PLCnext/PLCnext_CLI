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
using System.Text;
using Autofac;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.IO;
using PlcNext.NamedPipeServer.CommandLine;
using PlcNext.NamedPipeServer.Communication;

namespace PlcNext.NamedPipeServer
{
    internal class BaseDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => PageStreamFactory.CreateDefault()).As<StreamFactory>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<CliServer>().As<ICliServer>().SingleInstance();
            builder.RegisterType<ServerConnectionLifetimeScope>().AsSelf().AutoActivate();
            builder.RegisterType<CommandLineFacade>().As<ICommandLineFacade>().InstancePerLifetimeScope();
            builder.RegisterType<JsonMessageParser>().As<IMessageParser>().InstancePerLifetimeScope();
            builder.RegisterType<ThreadingHeart>().As<IHeart>().InstancePerLifetimeScope();
            builder.RegisterModule<MessageSenderDiModule>();
        }
    }
}
