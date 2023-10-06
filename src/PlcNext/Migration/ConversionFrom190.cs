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

namespace PlcNext.Migration
{
    internal sealed class ConversionFrom190 : IConversionStep
    {
        public Version BaseVersion => new Version(19, 0);

        public void Execute(string migrationDestination)
        {
            //nothing to do
        }
    }
}
