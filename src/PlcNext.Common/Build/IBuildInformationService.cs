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
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;

namespace PlcNext.Common.Build
{
    public interface IBuildInformationService
    {
        BuildSystemProperties RetrieveBuildSystemProperties(Entity rootEntity, Target projectTarget,
                                                            ChangeObservable observable);
    }
}
