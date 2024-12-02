#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Globalization;
using System.Linq;
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
            EntityKeys.ToLower,
            EntityKeys.DuplicateNameCheckKey,
            EntityKeys.ValidateIdentifierKey
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
                case EntityKeys.DuplicateNameCheckKey:
                    if (owner.Root.Hierarchy().GroupBy(e => e.Name).Any(group => group.Count() > 1))
                    {
                        throw new EntitiesWithSameNameException(
                            owner.Root.Hierarchy().GroupBy(e => e.Name)
                                                  .First(group => group.Count() > 1)
                                                  .First().Name);
                    }
                    return owner.Create(key, string.Empty);
                case EntityKeys.ValidateIdentifierKey:
                    if (owner.Value<string>()?.Split('.').Any(x => IecKeywordsNotAllowedAsIdentifier.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase))) == true)
                    {
                        throw new KeywordUsedAsIdentifierException(owner.Value<string>()?
                                                                        .Split('.')
                                                                        .Where(x => IecKeywordsNotAllowedAsIdentifier.Where(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()!= null)
                                                                        .FirstOrDefault());
                    }
                    return owner;
                    
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

        public static readonly string[] IecKeywordsNotAllowedAsIdentifier = new[]
        {
            "ABSTRACT",
            "ACTION",
            "ARRAY",
            "AT",
            "BY",
            "CASE",
            "CLASS",
            "CONFIGURATION",
            "CONSTANT",
            "DO",
            "EN",
            "END_ACTION",
            "END_CASE",
            "END_CLASS",
            "END_CONFIGURATION",
            "END_FOR",
            "END_FUNCTION",
            "END_FUNCTION_BLOCK",
            "END_IF",
            "END_INTERFACE",
            "END_METHOD",
            "END_PROGRAM",
            "END_REPEAT",
            "END_RESOURCE",
            "END_STEP",
            "END_STRUCT",
            "END_TRANSITION",
            "END_TYPE",
            "END_VAR",
            "END_WHILE",
            "ENO",
            "ELSE",
            "ELSIF",
            "EXIT",
            "EXTENDS",
            "FALSE",
            "FINAL",
            "FOR",
            "FROM",
            "FUNCTION",
            "FUNCTION_BLOCK",
            "F_EDGE",
            "IF",
            "IMPLEMENTS",
            "INITIAL_STEP",
            "INTERFACE",
            "INTERNAL",
            "METHOD",
            "NON_RETAIN",
            "OF",
            "ON",
            "OVERRIDE",
            "PROGRAM",
            "PUBLIC",
            "PRIVATE",
            "PROTECTED",
            "READ_ONLY",
            "READ_WRITE",
            "REPEAT",
            "RESOURCE",
            "RETAIN",
            "RETURN",
            "R_EDGE",
            "STEP",
            "STRUCT",
            "TASK",
            "THEN",
            "TO",
            "TRANSITION",
            "TRUE",
            "TYPE",
            "UNTIL",
            "VAR",
            "VAR_INPUT",
            "VAR_OUTPUT",
            "VAR_IN_OUT",
            "VAR_TEMP",
            "VAR_EXTERNAL",
            "VAR_ACCESS",
            "VAR_CONFIG",
            "VAR_GLOBAL",
            "WHILE",
            "WITH",
            "SINT",
            "INT",
            "DINT",
            "LINT",
            "USINT",
            "UINT",
            "UDINT",
            "ULINT",
            "BOOL",
            "BYTE",
            "WORD",
            "DWORD",
            "LWORD",
            "REAL",
            "LREAL",
            "STRING",
            "WSTRING",
            "CHAR",
            "WCHAR",
            "TIME",
            "LTIME",
            "DATE",
            "LDATE",
            "TIME_OF_DAY",
            "LTIME_OF_DAY",
            "DATE_AND_TIME",
            "LDATE_AND_TIME",
            "ANY",
            "ANY_DERIVED",
            "ANY_ELEMENTARY",
            "ANY_MAGNITUDE",
            "ANY_NUM",
            "ANY_REAL",
            "ANY_INT",
            "ANY_BIT",
            "ANY_STRING",
            "ANY_DATE",
            "ELEMENTARYSAFEANY",
            "ANY_SAFEDERIVED",
            "ANY_SAFEELEMENTARY",
            "ANY_SAFEMAGNITUDE",
            "ANY_SAFENUM",
            "ANY_SAFEREAL",
            "ANY_SAFEINT",
            "ANY_SAFEBIT",
            "ANY_SAFESTRING",
            "ANY_SAFEDATE",
            "SAFEELEMENTARYSAFEBOOL",
            "SAFEBYTE",
            "SAFEWORD",
            "SAFEDWORD",
            "SAFELWORD",
            "SAFESINT",
            "SAFEINT",
            "SAFEDINT",
            "SAFELINT",
            "SAFEUSINT",
            "SAFEUINT",
            "SAFEUDINT",
            "SAFEULINT",
            "SAFEREAL",
            "SAFELREAL",
            "SAFESTRING",
            "SAFEWSTRING",
            "SAFECHAR",
            "SAFEWCHAR",
            "SAFETIME",
            "SAFELTIME",
            "SAFET",
            "SAFELT",
            "SAFEDATE",
            "SAFELDATE",
            "SAFED",
            "SAFELD",
            "SAFETIME_OF_DAY",
            "SAFELTIME_OF_DAY",
            "SAFETOD",
            "SAFELTOD",
            "SAFEDATE_AND_TIME",
            "SAFELDATE_AND_TIME",
            "SAFEDT",
            "SAFELDTSAFETRUE",
            "SAFEFALSEANALOG",
            "SAFEANALOGTHIS",
            "SUPERCONTINUEANY_SIGNED",
            "ANY_UNSIGNEDANY_SAFESIGNED",
            "ANY_SAFEUNSIGNEDNAMESPACE",
            "END_NAMESPACE",
            "USING",
            "AND",
            "OR",
            "XOR",
            "MOD",
            "NOT",
            "AND_S",
            "OR_S",
            "XOR_S",
            "MOD_S",
            "NOT_S",
        };
    }
}
