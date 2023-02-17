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
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools;

namespace PlcNext.Common.Deploy
{
    internal class SharedNativeLibraryBuilderDeployStep : IDeployStep
    {
        private readonly ILibraryBuilderExecuter builder;

        public SharedNativeLibraryBuilderDeployStep(ILibraryBuilderExecuter builder)
        {
            this.builder = builder;
        }

        public string Identifier => "SharedNativeLibraryBuilderDeployStep";

        public void Execute(Entity dataModel, ChangeObservable observable)
        {
            int exitCode = builder.ExecuteSn(dataModel);
            if (exitCode != 0)
                throw new FormattableException("Deploying library failed!");
        }
    }
}
