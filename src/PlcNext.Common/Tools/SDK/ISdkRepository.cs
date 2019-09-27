#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Tools.SDK
{
    public interface ISdkRepository
    {
        IEnumerable<SdkInformation> Sdks { get; }
        SdkInformation GetSdk(Target target);
        IEnumerable<Target> GetAllTargets();
        IEnumerable<string> SdkPaths { get; }
        Task Update(string sdkPath, bool forced = false);
        void Remove(string sdkPath);
        event EventHandler<EventArgs> Loaded;
        event EventHandler<EventArgs> Updated;
    }
}
