#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace Test.PlcNext.Tools.XUnit
{
    public class SkipTestException : Exception
    {
        public SkipTestException(string reason)
            : base(reason) { }
    }
}
