#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;
using System.IO;
using PlcNext.Common.Tools.FileSystem;

namespace Test.PlcNext.Tools.Abstractions
{
    internal interface IFileSystemAbstraction : IAbstraction
    {
        Stream Open(string path);

        VirtualDirectory CurrentDirectory { get; }

        bool FileExists(string path, string basePath);

        void Load(string workspace);
        void Load(Stream content, string fileName);
        void LoadInto(string workspace, string destination);

        IEnumerable<string> CreatedFiles { get; }

        IEnumerable<string> ChangedFiles { get; }
        IEnumerable<string> DeletedFiles { get; }
        IFileSystem FileSystem { get; }
        void ThrowOnAccess(string path);
        void SetCurrentDirectory(string path);
        void RemoveSdk();
        bool FindFile(ref string path);
        VirtualDirectory GetTemporaryFolder(bool createNew = true);
        void RemoveApplicationFile(string fileName);
        string MoveApplicationFile(string fileName, string newLocation);
        Stream GetDeletedFileStream(string commandArgsFile);
    }
}
