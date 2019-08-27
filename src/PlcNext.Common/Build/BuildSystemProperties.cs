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

namespace PlcNext.Common.Build
{
    public class BuildSystemProperties
    {
        public BuildSystemProperties(IEnumerable<string> includePaths)
        {
            IncludePaths = includePaths;
        }

        public IEnumerable<string> IncludePaths { get; }
    }
}
