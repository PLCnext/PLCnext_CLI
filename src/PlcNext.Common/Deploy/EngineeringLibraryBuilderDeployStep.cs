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
using System.Linq;
using System.Text;
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.DynamicCommands;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Deploy
{
    internal class EngineeringLibraryBuilderDeployStep : IDeployStep
    {
        private readonly ILibraryBuilderExecuter builder;

        public EngineeringLibraryBuilderDeployStep(ILibraryBuilderExecuter builder)
        {
            this.builder = builder;
        }

        public string Identifier => "LibraryBuilderDeployStep";

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            builder.Execute(dataModel);
        }
    }
}
