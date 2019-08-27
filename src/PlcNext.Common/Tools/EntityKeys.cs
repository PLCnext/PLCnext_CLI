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

namespace PlcNext.Common.Tools
{
    public static class EntityKeys
    {
        //TemplateContentProvider
        public const string TemplateKey = "template";
        public const string TemplateFilesKey = "files";
        public const string HiearchyKey = "hierarchy";
        public const string RelatedKey = "related";

        //RootContentProvider
        public const string RootKey = "root";
        public const string EscapeProjectNameFormatKey = "escapeprojectname";

        //ConstantContentProvider
        public const string NewlineKey = "newline";
        public const string ActiveDirectoryKey = "active-directory";
        public const string IsRootedKey = "isrooted";
        public const string InternalDirectoryKey = "__internaldirectory__";

        //SettingsContentProvider
        public const string SettingsKey = "settings";

        //CodeModelContentProvider
        public const string PortStructsKey = "portstructs";
        public const string PortEnumsKey = "portenums";
        public const string FieldsKey = "fields";
        public const string BaseTypeKey = "basetype";
        public const string FieldNameKey = "fieldname";
        public const string FieldArpDataTypeKey = "arpdatatype";
        public const string DataTypeKey = "datatype";
        public const string MultiplicityKey = "multiplicity";
        public const string TypeMetaDataFormatKey = "typemetadataformat";
        public const string FileKey = "file";
        public const string PortAttributeKey = "port";
        public const string BaseDirectoryKey = "base-directory";

        //CppContentProvider
        public const string IncludeKey = "include";

        //ProjectSettingsProvider
        public const string ProjectSettingsKey = "__internalprojectsettings__";
        public const string ProjectVersionKey = "__internalprojectversion__";

        //General keys
        public const string PathKey = "path";
        public const string SourceDirectoryKey = "sources";
        public const string IncludeDirectoryKey = "includes";
        public const string OutputKey = "output";
        public const string NameKey = "name";
        public const string FullNameKey = "fullname";
        public const string NamespaceKey = "namespace";
        public const string FormatKey = "format";
        public const string ValueKey = "value";

        //MetaDataKeys
        public const string IsTemplateOnly = "IsTemplateOnly";
        public const string IsRoot = "IsRoot";
    }
}
