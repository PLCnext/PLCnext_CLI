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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.Build
{
    internal class CMakeContentProvider : IEntityContentProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly Regex projectNameFinder = new Regex(@"(?<!\S)project\((?<name>[^\s\)]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CMakeContentProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.IsRoot() && key == EntityKeys.NameKey && GetCMakeFile(owner) != null;
        }

        private VirtualFile GetCMakeFile(Entity owner)
        {
            if (owner.HasPath)
            {
                string basePath = owner.Path;
                if (fileSystem.FileExists(Constants.CMakeFileName, basePath))
                {
                    return fileSystem.GetFile(Constants.CMakeFileName, basePath);
                }
            }

            return null;
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            VirtualFile cmakeFile = GetCMakeFile(owner);
            using (Stream fileStream = cmakeFile.OpenRead())
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    Match projectNameMatch = projectNameFinder.Match(line);
                    if (projectNameMatch.Success)
                    {
                        return owner.Create(key, projectNameMatch.Groups["name"].Value);
                    }
                }
            }

            return owner.Create(key, Path.GetFileName(owner.Path));
        }
    }
}
