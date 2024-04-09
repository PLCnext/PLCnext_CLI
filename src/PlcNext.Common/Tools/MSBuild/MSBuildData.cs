#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Tools.MSBuild
{
    public class MSBuildData
    {
        public MSBuildData(string path)
        {
            FullName = path;
        }

        public string FullName { get; set; }
        public string Parameters { get; set; } = string.Empty;
    }
}
