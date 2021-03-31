#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using PlcNext.Common.Tools;
using System.Collections.Generic;
using PlcNext.Common.DataModel;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.UI;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Build
{
    internal interface ILibraryBuilderExecuter
    {
        int Execute(ProjectEntity project, string metaFilesDirectory, string libraryLocation, string outputDirectory,
                    ChangeObservable changeObservable, IUserInterface userInterface, Guid libraryGuid,
                    IEnumerable<(Target, string)> targets,
                    Dictionary<Target, IEnumerable<VirtualFile>> externalLibraries, string buildType);

        int Execute(Entity dataModel);

        int ExecuteAcf(Entity dataModel);
    }
}