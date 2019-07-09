#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;

namespace PlcNext.Common.Tools.FileSystem
{
    public interface IFileContentResolver : IEntryContentResolver
    {
        Stream GetContent(bool write = false, bool retry = false);
        void Touch();
        DateTime LastWriteTime { get; }
        bool CheckAccess();
    }
}
