#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using NSubstitute;
using PlcNext.Common.Tools.IO;
using System;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedPasswordProviderAbstraction : IPasswordProviderAbstraction
    {
        private readonly IPasswordProvider passwordProvider = Substitute.For<IPasswordProvider>();

        public MockedPasswordProviderAbstraction()
        {
            passwordProvider.ProvidePassword().Returns("testpassword");
        }

        public void Dispose()
        {
        }

        public void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage)
        {
            exportProvider.AddInstance(passwordProvider);
        }
    }
}
