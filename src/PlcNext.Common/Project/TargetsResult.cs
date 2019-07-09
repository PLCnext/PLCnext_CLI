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
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Project
{
    internal class TargetsResult
    {
        public TargetsResult(IEnumerable<Target> validTargets, IEnumerable<Exception> errors)
        {
            ValidTargets = validTargets;
            Errors = errors;
        }

        public IEnumerable<Target> ValidTargets { get; }
        public IEnumerable<Exception> Errors { get; }
    }
}
