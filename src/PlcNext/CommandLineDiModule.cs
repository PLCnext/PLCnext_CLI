#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;
using PlcNext.CommandLine;
using PlcNext.Common.CommandLine;

namespace PlcNext
{
    public class CommandLineDiModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CollectiveDynamicVerbsFactory>().As<IDynamicVerbFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CommandLineParser>().As<ICommandLineParser>().InstancePerLifetimeScope();
        }
    }
}