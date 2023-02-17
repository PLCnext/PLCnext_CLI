#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.IO
{
    internal interface IDirectoryPackService
    {
        void Pack(VirtualDirectory directory, string destination, ChangeObservable observable = null);
    }
}
