#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using PlcNext.Common.Tools.UI;

namespace Test.PlcNext.Tools.Abstractions
{
    internal interface IExceptionHandlerAbstraction : IAbstraction
    {
        IUserInterface UserInterface { get; set; }
        bool WasExceptionThrown(Type exceptionType);
    }
}
