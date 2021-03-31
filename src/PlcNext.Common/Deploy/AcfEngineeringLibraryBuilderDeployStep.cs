#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Build;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Deploy
{
    internal class AcfEngineeringLibraryBuilderDeployStep : IDeployStep
    {
        private readonly ILibraryBuilderExecuter builder;

        public AcfEngineeringLibraryBuilderDeployStep(ILibraryBuilderExecuter builder)
        {
            this.builder = builder;
        }

        public string Identifier => "AcfLibraryBuilderDeployStep";

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            int exitCode = builder.ExecuteAcf(dataModel);
            if (exitCode != 0)
                throw new FormattableException("Deploying library failed!");
        }
    }
}
