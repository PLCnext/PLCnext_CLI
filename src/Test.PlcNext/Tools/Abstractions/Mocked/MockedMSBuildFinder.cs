#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using NSubstitute;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedMSBuildFinder : IMSBuildFinderAbstraction
    {
        private readonly IMSBuildFinder msBuildFinder = Substitute.For<IMSBuildFinder>();

        public MockedMSBuildFinder()
        {
            msBuildFinder.FindMSBuild(Arg.Any<Entity>()).Returns(new MSBuildData("test"));
        }

        public void Dispose()
        {

        }
        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            exportProvider.AddInstance(msBuildFinder);
        }
    }
}
