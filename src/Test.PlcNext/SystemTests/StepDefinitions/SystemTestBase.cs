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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.Tools;
using Test.PlcNext.Tools;
using Xunit.Abstractions;

namespace Test.PlcNext.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase : FeatureFixture, IDisposable
    {
        private readonly SystemTestContext testContext;

        private SystemTestContext ScenarioContext
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

        protected SystemTestBase(SystemTestContext testContext, ITestOutputHelper helper) : base(helper)
        {
            this.testContext = testContext;
        }

        protected bool IsWindowsSystem => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        
        protected string Adapt(string path) => IsWindowsSystem?path.Replace('/','\\'):path;

        public void Dispose()
        {
            ScenarioExtensions.RunWithTimeout(() => testContext?.Dispose(), 1000, false);
        }
    }
}
