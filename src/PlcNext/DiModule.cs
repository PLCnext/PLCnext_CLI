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

namespace PlcNext
{
    public class DiModule : Module
    {
        private readonly bool noSdkExploration;
        private readonly bool useAgents;

        public DiModule(bool noSdkExploration, bool useAgents)
        {
            this.noSdkExploration = noSdkExploration;
            this.useAgents = useAgents;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new Common.DiModule(noSdkExploration));
            builder.RegisterModule(new CppParser.DiModule(useAgents));
            builder.RegisterModule<CommandLineDiModule>();
            builder.RegisterType<MessageBoard>().As<IMessageBoard>().InstancePerLifetimeScope();
        }
    }
}
