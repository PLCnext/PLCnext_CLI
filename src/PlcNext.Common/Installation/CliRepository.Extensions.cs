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

namespace PlcNext.Common.Installation
{
    public partial class CliVersionDefinition
    {
        public string GetInformalVersion()
        {
            return string.IsNullOrEmpty(informalversion) ? version : informalversion;
        }
    }
}
