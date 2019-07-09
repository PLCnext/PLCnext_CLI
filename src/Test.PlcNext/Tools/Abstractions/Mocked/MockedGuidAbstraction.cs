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
using NSubstitute;
using PlcNext.Common.Tools;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedGuidAbstraction : IGuidFactoryAbstraction
    {
        private readonly IGuidFactory guidFactory = Substitute.For<IGuidFactory>();

        public MockedGuidAbstraction()
        {
            guidFactory.Create().Returns(Guid.Parse("89b2c2bc-b296-4ab4-a057-b6add1e37138"));
        }

        public void Dispose()
        {
            
        }

        public void Initialize(InstancesRegistrationSource exportProvider)
        {
            exportProvider.AddInstance(guidFactory);
        }
    }
}
