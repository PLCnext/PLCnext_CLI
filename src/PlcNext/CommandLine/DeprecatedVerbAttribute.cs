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

namespace PlcNext.CommandLine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DeprecatedVerbAttribute : Attribute
    {
        public DeprecatedVerbAttribute(string alternativeVerb)
        {
            AlternativeVerb = alternativeVerb;
        }

        public string AlternativeVerb { get; }
    }
}
