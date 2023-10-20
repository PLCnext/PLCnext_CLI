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
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Tools.Web;

namespace PlcNext.Common
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
            builder.RegisterModule(new BaseDiModule(noSdkExploration));
            builder.RegisterType<ConsoleUserInterface>().As<IUserInterface>().InstancePerLifetimeScope();
            builder.RegisterType<FileBaseFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<ProcessManager>().As<IProcessManager>().As<IProcessInformationService>()
                   .InstancePerLifetimeScope().PropertiesAutowired();
            builder.RegisterType<ConsoleProgressVisualizer>().As<IProgressVisualizer>().InstancePerLifetimeScope();
            builder.RegisterType<RealEnvironmentService>().As<IEnvironmentService>().InstancePerLifetimeScope();
            builder.RegisterType<FormattableExceptionHandler>().As<IExceptionHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GuidFactory>().As<IGuidFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CMakeConversation>().As<ICMakeConversation>().InstancePerLifetimeScope();
            builder.RegisterType<CMakeSdkExplorer>().As<ISdkExplorer>().InstancePerLifetimeScope();
        }
    }
}
