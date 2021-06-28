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
using PlcNext.Common.CodeModel;
using PlcNext.CppParser.CppRipper;
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Includes;
using PlcNext.CppParser.IncludeManager;

namespace PlcNext.CppParser
{
    public class DiModule : Module
    {
        private readonly bool useAgents;

        public DiModule(bool useAgents)
        {
            this.useAgents = useAgents;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CppCodeLanguage>().As<ICodeLanguage>().InstancePerLifetimeScope();
            if (useAgents)
            {
                builder.RegisterModule<AgentsModule>();
            }
            else
            {
                builder.RegisterType<CppRipper.CppRipper>().As<IParser>().InstancePerLifetimeScope();
                builder.RegisterType<CppFileParser>().As<IFileParser>().InstancePerLifetimeScope();
                builder.RegisterType<CppIncludeManager>().As<IIncludeManager>().InstancePerLifetimeScope();
                builder.RegisterType<JsonIncludeCache>().As<IIncludeCache>().InstancePerLifetimeScope();
            }
        }
    }
}
