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
using System.IO;
using System.Text;
using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using PlcNext.Common.Tools.FileSystem;

namespace PlcNext.Common
{
    internal static class MetaExtensions
    {
        public static VirtualFile GetFile(this IType type, Entity dataSource)
        {
            return dataSource.Root.Value<ICodeModel>().Types[type];
        }
        public static VirtualFile GetFile(this IField field, Entity dataSource)
        {
            return dataSource.Root.Value<ICodeModel>().Types[field.ContainingType];
        }

        public static string GetRelativePath(this string path, string basePath)
        {
            if (!Path.IsPathRooted(path) || !Path.IsPathRooted(basePath))
            {
                return path;
            }
            Uri pathUri = new Uri(path, UriKind.Absolute);

            // Folders must end in a slash
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), 
                                   StringComparison.Ordinal))
            {
                basePath += Path.DirectorySeparatorChar;
            }
            Uri folderUri = new Uri(basePath, UriKind.Absolute);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string Singular(this string value)
        {
            if (value.EndsWith("s", StringComparison.Ordinal))
            {
                return value.EndsWith("ies", StringComparison.Ordinal)
                           ? $"{value.Substring(0, value.Length - 3)}y"
                           : value.Substring(0, value.Length - 1);
            }

            return value;
        }

        public static string Plural(this string value)
        {
            return value.EndsWith("y", StringComparison.Ordinal)
                       ? $"{value.Substring(0, value.Length - 1)}ies"
                       : $"{value}s";
        }
    }
}
