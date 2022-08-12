#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.DataModel;

namespace PlcNext.Common.Build
{
    internal interface ILibraryBuilderExecuter
    {
        int Execute(Entity dataModel);

        int ExecuteAcf(Entity dataModel);
    }
}