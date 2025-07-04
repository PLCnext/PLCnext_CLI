﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac;
using PlcNext;
using PlcNext.Common;

namespace Test.PlcNext.SystemTests.Tools
{
    internal class DiModule : Module
    {
        private readonly InstancesRegistrationSource registrationSource;
        private readonly bool autoActivatedComponents;

        public DiModule(InstancesRegistrationSource registrationSource, bool autoActivatedComponents)
        {
            this.registrationSource = registrationSource;
            this.autoActivatedComponents = autoActivatedComponents;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSource(registrationSource);
            builder.RegisterModule(new BaseDiModule(false, autoActivatedComponents));
            builder.RegisterModule(new global::PlcNext.CppParser.DiModule());
            builder.RegisterModule<CommandLineDiModule>();
        }
    }
}
