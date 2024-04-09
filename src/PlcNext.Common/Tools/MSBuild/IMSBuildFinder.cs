#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.DataModel;

namespace PlcNext.Common.Tools.MSBuild
{
    public interface IMSBuildFinder
    {
        MSBuildData FindMSBuild(Entity rootEntity);
    }
}
