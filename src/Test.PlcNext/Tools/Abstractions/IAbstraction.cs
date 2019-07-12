#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions
{
    public interface IAbstraction : IDisposable
    {
        void Initialize(InstancesRegistrationSource exportProvider, Action<string> printMessage);
    }
}