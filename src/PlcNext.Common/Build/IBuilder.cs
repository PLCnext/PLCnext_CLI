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
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Build
{
    internal interface IBuilder
    {
        void Build(BuildInformation buildInfo, ChangeObservable observable, IEnumerable<string> targets);

        int BuildLibraryForProject(Entity project, ChangeObservable observable, string metaFilesDirectory,
                                   string libraryLocation, string outputDirectory, Guid libraryGuid,
                                   IEnumerable<string> targets, IEnumerable<string> externalLibraries, string buildType);
    }
}
