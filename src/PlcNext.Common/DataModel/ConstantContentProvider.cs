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
using System.Linq;
using System.Text;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.DataModel
{
    internal class ConstantContentProvider : IEntityContentProvider
    {
        private readonly IFileSystem fileSystem;

        private static readonly string[] Resolvables = new[]
        {
            EntityKeys.NewlineKey,
            EntityKeys.ActiveDirectoryKey,
            EntityKeys.IsRootedKey,
            EntityKeys.InternalDirectoryKey
        };

        public ConstantContentProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return Resolvables.Contains(key) ||
                   (key == EntityKeys.InternalDirectoryKey && owner.HasPath) ||
                   owner.Value<VirtualFile>()?.HasPropertyValueEntity(MapFileAccessKey(key)) == true ||
                   (owner.Value<VirtualDirectory>()?.HasPropertyValueEntity(MapFileAccessKey(key)) == true &&
                    fallback);
        }

        private string MapFileAccessKey(string key)
        {
            if (key == EntityKeys.PathKey)
            {
                return EntityKeys.FullNameKey;
            }

            return key;
        }

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            VirtualFile file = owner.Value<VirtualFile>();
            if (file != null)
            {
                if (key == EntityKeys.PathKey)
                {
                    return owner.Create(key, file.Parent.FullName);
                }

                return owner.PropertyValueEntity(MapFileAccessKey(key), file, key);
            }

            VirtualDirectory directory = owner.Value<VirtualDirectory>();
            if (directory != null)
            {
                if (key == EntityKeys.PathKey)
                {
                    return owner.Create(key, directory.FullName);
                }

                return owner.PropertyValueEntity(MapFileAccessKey(key), directory, key);
            }

            switch (key)
            {
                case EntityKeys.NewlineKey:
                    return owner.Create(key, Environment.NewLine);
                case EntityKeys.ActiveDirectoryKey:
                    return owner.Create(key, fileSystem.CurrentDirectory.FullName);
                case EntityKeys.IsRootedKey:
                    bool isRooted = fileSystem.IsRooted(owner.Value<string>());
                    return owner.Create(key, isRooted.ToString(), isRooted);
                case EntityKeys.InternalDirectoryKey:
                    return owner.Create(key, fileSystem.GetDirectory(owner.Path, createNew: false));
                default:
                    throw new ContentProviderException(key, owner);
            }
        }
    }
}
