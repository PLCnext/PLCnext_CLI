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

namespace PlcNext.Common.Tools.FileSystem
{
    public interface IEntryContentResolver
    {
        string FullName { get; }
        bool Created { get; }

        void Delete();

        void UnDelete();

        string CreatePath(string[] parts);

        string[] SplitPath(string path);
    }
}
