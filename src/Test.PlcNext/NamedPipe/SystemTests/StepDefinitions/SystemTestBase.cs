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
using Test.PlcNext.NamedPipe.Tools;

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

        protected SystemTestBase()
        {
            testContext = new SystemTestContext();
        }

        protected SystemTestBase(SystemTestContext testContext)
        {
            this.testContext = testContext;
        }

        public void Dispose()
        {
            testContext?.Dispose();
        }
    }
}
