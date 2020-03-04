#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using Xunit;
using Xunit.Sdk;

namespace Test.PlcNext.Tools.XUnit
{
    [XunitTestCaseDiscoverer("Test.PlcNext.Tools.XUnit.XunitExtensions.SkippableFactDiscoverer", "Test.PlcNext")]
    public class SkippableFactAttribute : FactAttribute { }
}
