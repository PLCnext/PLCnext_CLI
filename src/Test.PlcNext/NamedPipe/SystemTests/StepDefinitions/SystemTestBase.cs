#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics;
using LightBDD.XUnit2;
using PlcNext.Common.Tools;
using Test.PlcNext.NamedPipe.Tools;
using Xunit.Abstractions;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase : FeatureFixture, IDisposable
    {
        private readonly SystemTestContext testContext;

        protected SystemTestContext ScenarioContext
        {
            get
            {
                if (!testContext.Initialized)
                {
                    testContext.Initialize((message) =>
                    {
                        TestOutput.WriteLine(message);
                        Debug.WriteLine(message);
                    });
                }
                return testContext;
            }
        }

        protected SystemTestBase(ITestOutputHelper output) : base(output)
        {
            testContext = new SystemTestContext();
        }

        protected SystemTestBase(SystemTestContext testContext, ITestOutputHelper output) : base(output)
        {
            this.testContext = testContext;
        }

        public void Dispose()
        {
            Extensions.ExecutesWithTimeout(() => testContext?.Dispose(), 2000);
        }
    }
}
