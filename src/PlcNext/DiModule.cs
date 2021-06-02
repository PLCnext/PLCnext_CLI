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
using Agents.Net;
using Autofac;
using PlcNext.CliNamedPipeMediator;
using PlcNext.Common.CommandLine;

namespace PlcNext
{
    public class DiModule : Module
    {
        private readonly bool noSdkExploration;

        public DiModule(bool noSdkExploration)
        {
            this.noSdkExploration = noSdkExploration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Common.DiModule(noSdkExploration));
            builder.RegisterModule<CppParser.DiModule>();
            builder.RegisterModule<UpdateDiModule>();
            builder.RegisterModule<CommandLineDiModule>();
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
        }
    }
}
