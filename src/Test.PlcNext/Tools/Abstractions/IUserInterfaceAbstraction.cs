#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace Test.PlcNext.Tools.Abstractions
{
    internal interface IUserInterfaceAbstraction : IAbstraction
    {
        IEnumerable<string> Errors { get; }
        IEnumerable<string> Informations { get; }
        IEnumerable<string> Warnings { get; }
    }
}
