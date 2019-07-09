#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Build
{
    internal interface IBuildExecuter
    {
        void ExecuteBuild(BuildInformation buildInfo, ChangeObservable changeObservable);
    }
}
