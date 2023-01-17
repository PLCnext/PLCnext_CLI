#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Autofac.Features.AttributeFilters;
using PlcNext.Common.Tools.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Build
{
    internal class SharedNativeBuilder : IBuilder
    {
        private readonly IBuilder baseBuilder;

        public SharedNativeBuilder([KeyFilter("DefaultBuildEngine")] IBuilder baseBuilder)
        {
            this.baseBuilder = baseBuilder;
        }
        public void Build(BuildInformation buildInfo, ChangeObservable observable, IEnumerable<string> targets)
        {
            buildInfo.BuildProperties += "-DECLR_VERSION=v3.3.0";
            baseBuilder.Build(buildInfo, observable, targets);
        }
    }
}
