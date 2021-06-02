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

namespace PlcNext.CppParser
{
    public class DiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CppCodeLanguage>().As<ICodeLanguage>().InstancePerLifetimeScope();
            builder.RegisterModule<AgentsModule>();
        }
    }
}
