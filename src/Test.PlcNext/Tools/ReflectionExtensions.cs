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
using System.Linq;
using System.Reflection;
using System.Text;
using PlcNext.Common.Tools.FileSystem;

namespace Test.PlcNext.Tools
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<VirtualEntry> Entries(this VirtualDirectory directory)
        {
            PropertyInfo property = typeof(VirtualDirectory).GetProperty("Entries", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (IEnumerable<VirtualEntry>) (property?.GetValue(directory) ?? Enumerable.Empty<VirtualEntry>());
        }
    }
}
