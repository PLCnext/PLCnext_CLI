#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Tools.SDK
{
    public class IncludePath
    {
        public IncludePath(string pathValue, bool? exists = null, IEnumerable<Target> targets = null)
        {
            Directory = null;
            PathValue = pathValue;
            Exists = exists;

            if (targets == null)
                Targets = Enumerable.Empty<Target>();
            else
                Targets = targets;
        }
        public string PathValue { get; }
        public bool? Exists { get; set; }
        public IEnumerable<Target> Targets { get; set; }
        public VirtualDirectory Directory { get; set; }
    }
}
