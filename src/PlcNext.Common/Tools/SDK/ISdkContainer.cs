#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.SDK
{
    internal interface ISdkContainer
    {
        bool Contains(string path);
        void Remove(string sdkRootPath);
        void Add(string sdkRootPath, SdkSchema sdkSchema);
        SdkInformation Get(string sdkRootPath);
    }
}