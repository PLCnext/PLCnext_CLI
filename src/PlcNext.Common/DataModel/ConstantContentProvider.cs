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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common.DataModel
{
    internal class ConstantContentProvider : PriorityContentProvider
    {
        private readonly IFileSystem fileSystem;

        private static readonly string[] Resolvables = new[]
        {
            EntityKeys.NewlineKey,
            EntityKeys.ActiveDirectoryKey,
            EntityKeys.IsRootedKey,
            EntityKeys.InternalDirectoryKey,
            EntityKeys.InternalTempDirectoryKey,
            EntityKeys.ChunkStartKey,
            EntityKeys.ChunkEndKey, 
            EntityKeys.CountKey,
            EntityKeys.IncrementKey,
            EntityKeys.DecrementKey,
            EntityKeys.NegateKey,
            EntityKeys.OriginKey,
            EntityKeys.ThrowIfMultidimensionalKey,
            EntityKeys.ContainsLtGt,
            EntityKeys.IsEmpty,
            EntityKeys.ToUpper,
            EntityKeys.ToLower
        };

        public ConstantContentProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        private VirtualDirectory tempDirectory;
        private VirtualDirectory TempDirectory => tempDirectory ?? (tempDirectory = fileSystem.GetTemporaryDirectory());

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return Resolvables.Contains(key) ||
                   (key == EntityKeys.InternalDirectoryKey && owner.HasPath) ||
                   owner.Value<VirtualFile>()?.HasPropertyValueEntity(MapFileAccessKey(key)) == true ||
                   (owner.Value<VirtualDirectory>()?.HasPropertyValueEntity(MapFileAccessKey(key)) == true &&
                    fallback);
        }

        private static string MapFileAccessKey(string key)
        {
            if (key == EntityKeys.PathKey)
            {
                return EntityKeys.FullNameKey;
            }

            return key;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            VirtualFile file = owner.Value<VirtualFile>();
            if (file != null)
            {
                if (key == EntityKeys.PathKey)
                {
                    return owner.Create(key, file.Parent.FullName);
                }

                if (file.HasPropertyValueEntity(MapFileAccessKey(key)))
                {
                    return owner.PropertyValueEntity(MapFileAccessKey(key), file, key);
                }
            }

            VirtualDirectory directory = owner.Value<VirtualDirectory>();
            if (directory != null)
            {
                if (key == EntityKeys.PathKey)
                {
                    return owner.Create(key, directory.FullName);
                }

                if (directory.HasPropertyValueEntity(MapFileAccessKey(key)))
                {
                    return owner.PropertyValueEntity(MapFileAccessKey(key), directory, key);
                }
            }

            switch (key)
            {
                case EntityKeys.NewlineKey:
                    return owner.Create(key, Environment.NewLine);
                case EntityKeys.ActiveDirectoryKey:
                    return owner.Create(key, fileSystem.CurrentDirectory.FullName);
                case EntityKeys.IsRootedKey:
                    bool isRooted = fileSystem.IsRooted(owner.Value<string>());
                    return owner.Create(key, isRooted.ToString(CultureInfo.InvariantCulture), isRooted);
                case EntityKeys.InternalDirectoryKey:
                    string path = owner.HasValue<string>() &&
                                  fileSystem.DirectoryExists(owner.Value<string>())
                                      ? owner.Value<string>()
                                      : owner.Path;
                    return owner.Create(key, fileSystem.GetDirectory(path, createNew: false));
                case EntityKeys.InternalTempDirectoryKey:
                    return owner.Create(key, TempDirectory);
                case EntityKeys.ChunkStartKey:
                    return owner.Create(key, owner.Value<DataChunk>().Start);
                case EntityKeys.ChunkEndKey:
                    return owner.Create(key, owner.Value<DataChunk>().End);
                case EntityKeys.CountKey:
                    return owner.Create(key, owner.Count.ToString(CultureInfo.InvariantCulture), owner.Count);
                case EntityKeys.IncrementKey:
                    return IncrementValue();
                case EntityKeys.DecrementKey:
                    return DecrementValue();
                case EntityKeys.NegateKey:
                    if(bool.TryParse(owner.Value<string>(), out bool value))
                    {
                        return owner.Create(key, (!value).ToString(CultureInfo.InvariantCulture), !value);
                    }
                    throw new ContentProviderException(key, owner);
                case EntityKeys.OriginKey:
                    return owner.Origin;
                case EntityKeys.ThrowIfMultidimensionalKey:
                    if (owner.Value<string>()?.Contains(',', StringComparison.Ordinal) == true)
                        throw new MultidimensionalArrayNotSupportedException();
                    return owner;
                case EntityKeys.ContainsLtGt:
                    return ContainsLtGt();
                case EntityKeys.IsEmpty:
                    return owner.Create(key, string.IsNullOrEmpty(owner.Value<string>()).ToString(CultureInfo.InvariantCulture), 
                                             string.IsNullOrEmpty(owner.Value<string>()));
                case EntityKeys.ToUpper:
                    return owner.Create(key, owner.Value<string>().ToUpperInvariant());
                case EntityKeys.ToLower:
#pragma warning disable CA1308 // Normalize strings to uppercase
                    return owner.Create(key, owner.Value<string>().ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
                default:
                    throw new ContentProviderException(key, owner);

                    Entity IncrementValue()
                    {
                        string number = owner.Value<string>();
                        if (number != null)
                        {
                            if (int.TryParse(number, out int result))
                            {
                                result++;
                                return owner.Create(key, result.ToString(CultureInfo.InvariantCulture), result);
                            }
                        }
                        
                        return owner.Create(key, number);
                    }

                    Entity DecrementValue()
                    {
                        string number = owner.Value<string>();
                        if (number != null)
                        {
                            if (int.TryParse(number, out int result))
                            {
                                result--;
                                return owner.Create(key, result.ToString(CultureInfo.InvariantCulture), result);
                            }
                        }

                        return owner.Create(key, number);
                    }
                    
                    Entity ContainsLtGt()
                    {
                        string value = owner.Value<string>();
                        bool result = value.Contains('<', StringComparison.Ordinal) || value.Contains('>', StringComparison.Ordinal);
                        return owner.Create(key, result.ToString(CultureInfo.InvariantCulture), result);
                        
                    }
            }
        }
    }
}
