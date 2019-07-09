#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Project
{
    public class ProjectSettingChange : Change
    {
        public ProjectSettingChange(Action invertAction, string path, string description = "") : base(invertAction, description)
        {
            Path = path;
        }

        public string Path { get; }
    }
}