#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Project
{
    internal interface ITargetParser
    {
        TargetsResult Targets(ProjectEntity project, bool validateTargets = true);
        string[] FormatTargets(IEnumerable<Target> targets, bool shortVersion);
        Target RemoveTarget(ProjectEntity project, string target, string version);
        Target AddTarget(ProjectEntity project, string target, string version);
        IEnumerable<(Target, string)> GetSpecificTargets(IEnumerable<string> targets, bool validate = true);
        void UpdateTargets(ProjectEntity project);
        Target ParseTarget(string target, string version, IEnumerable<Target> searchableTargets);
    }
}
