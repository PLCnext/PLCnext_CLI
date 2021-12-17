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
            builder.RegisterModule(new CppParser.DiModule());
            builder.RegisterModule<CommandLineDiModule>();
        }
    }
}
