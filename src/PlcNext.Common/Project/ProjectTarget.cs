#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcNext.Common.Project
{
    public class ProjectTarget
    {
        public ProjectTarget(Target target, bool isAvailable)
        {
            Target = target;
            IsAvailable = isAvailable;
        }

        public Target Target { get; }

        public bool IsAvailable { get; }
    }
}
