#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.Common.Tools.FileSystem
{
    /// <summary>
    /// Basic access to files and directories
    /// </summary>
    public interface IFileSystem
    {
        VirtualDirectory CurrentDirectory { get; }

        VirtualDirectory GetDirectory(string path, string basePath = "", bool createNew = true);

        VirtualDirectory GetTemporaryDirectory();

        bool DirectoryExists(string path, string basePath = "");

        VirtualFile GetFile(string path, string basePath = "");

        bool FileExists(string path, string basePath = "");

        string[] GetPath(string path);

        bool IsRooted(string path);
    }
}
