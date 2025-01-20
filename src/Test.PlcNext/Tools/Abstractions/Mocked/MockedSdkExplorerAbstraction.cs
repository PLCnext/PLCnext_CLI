#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using NSubstitute;
using PlcNext.Common.Tools.SDK;
using System;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedSdkExplorerAbstraction : ISdkExplorerAbstraction
    {
        private readonly ISdkExplorer sdkExplorer = Substitute.For<ISdkExplorer>();

        public MockedSdkExplorerAbstraction()
        {
            sdkExplorer.ExploreSdk(null, false)
                        .ReturnsForAnyArgs(Task.FromResult((SdkSchema)null));
        }

        public void FindTargetOnExplore(string name, string version)
        {
            SdkSchema sdkSchema = new SdkSchema();
            TargetSchema targetSchema = new TargetSchema();
            targetSchema.name = name;
            targetSchema.version = version;
            sdkSchema.Target = new[] { targetSchema };

            sdkExplorer.ExploreSdk(null, false)
                .ReturnsForAnyArgs(Task.FromResult(sdkSchema));
        }

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            exportProvider.AddInstance(sdkExplorer);
        }
        
        public void Dispose()
        {
        }
    }
}
