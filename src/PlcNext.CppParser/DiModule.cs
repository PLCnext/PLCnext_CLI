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
using PlcNext.CppParser.CppRipper.CodeModel;
using PlcNext.CppParser.CppRipper.CodeModel.Includes;

namespace PlcNext.CppParser
{
    public class DiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CppRipper.CppRipper>().As<IParser>().InstancePerLifetimeScope();
            builder.RegisterType<CppCodeLanguage>().As<ICodeLanguage>().InstancePerLifetimeScope();
            builder.RegisterType<CppFileParser>().As<IFileParser>().InstancePerLifetimeScope();
            builder.RegisterType<CppIncludeManager>().As<IIncludeManager>().InstancePerLifetimeScope();
            builder.RegisterType<JsonIncludeCache>().As<IIncludeCache>().InstancePerLifetimeScope();
        }
    }
}
