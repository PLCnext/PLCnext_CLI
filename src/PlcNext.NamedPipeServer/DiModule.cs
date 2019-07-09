#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;
using PlcNext.NamedPipeServer.CommandLine;
using PlcNext.NamedPipeServer.Tools;

namespace PlcNext.NamedPipeServer
{
    public class DiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<BaseDiModule>();
            builder.RegisterType<CommandLineBuilder>().As<ICommandLineBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<RealEnvironmentInformation>().As<IEnvironmentInformation>().InstancePerLifetimeScope();
        }
    }
}