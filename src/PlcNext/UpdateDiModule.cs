#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;
using PlcNext.NamedPipeServer.Tools;

namespace PlcNext
{
    public class UpdateDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<BaseUpdateDiModule>();
            builder.RegisterType<RealEnvironmentInformation>().As<IEnvironmentInformation>();
        }
    }
}