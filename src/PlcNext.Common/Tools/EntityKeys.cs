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
        public const string InternalTempDirectoryKey = "__internaltempdirectory__";
        public const string ChunkStartKey = "start";
        public const string ChunkEndKey = "end";
        public const string CountKey = "count";
        public const string IncrementKey = "increment";
        public const string DecrementKey = "decrement";
        public const string NegateKey = "negate";
        public const string OriginKey = "origin";
        public const string ContainsLtGt = "containsltgt";
        public const string IsEmpty = "isempty";
        public const string ToUpper = "toupper";
        public const string ToLower = "tolower";

        //SettingsContentProvider
        public const string SettingsKey = "settings";

        //CodeModelContentProvider
        public const string PortStructsKey = "portstructs";
        public const string PortAndTypeInformationStructs = "portandtypeinformationstructs";
        public const string PortEnumsKey = "portenums";
        public const string PortAndTypeInformationEnumsKey = "portandtypeinformationenums";
        public const string PortArraysKey = "portarrays";
        public const string PortAndTypeInformationArraysKey = "portandtypeinformationarrays";
        public const string VariablePortStringsKey = "variableportstrings";
        public const string VariablePortAndTypeInformationStringsKey = "variableportandtypeinformationstrings";
        public const string FieldsKey = "fields";
        public const string AttributesKey = "attributes";
        public const string BaseTypeKey = "basetype";
        public const string FieldNameKey = "fieldname";
        public const string FieldArpDataTypeKey = "arpdatatype";
        public const string DataTypeKey = "datatype";
        public const string MultiplicityKey = "multiplicity";
        public const string TypeMetaDataFormatKey = "typemetadataformat";
        public const string ExpandHiddenTypesFormatKey = "expandhiddentypes";
        public const string FilterHiddenTypesFormatKey = "filterhiddentypes";
        public const string FileKey = "file";
        public const string PortAttributeKey = "port";
        public const string TypeInformationAttributeKey = "typeinformation";
        public const string BaseDirectoryKey = "base-directory";
        public const string ResolvedTypeKey = "__internalresolvedtype__";
        public const string IsFieldKey = "isfield";
        public const string IsTypeKey = "istype";
        public const string BigDataProjectKey = "big-data-project";
        public const string NormalProjectKey = "normal-project";
        public const string IecDataTypeFormatKey = "iecdatatypeformat";
        public const string IecDataTypeNamespaceKey = "iecdatatypenamespace";
        public const string StringConstantReplaceKey = "stringconstantreplace";
        public const string MultiplicityConstantReplaceKey = "multiplicityconstantreplace";
        public const string IsArray = "isarray";
        public const string ThrowIfMultidimensionalKey = "throwifmultidimensional";
        public const string IECDataTypeAttributeNameKey = "iecdatatype";
        public const string ConvertToIECDatatypeKey = "converttoiecdatatype";

        //CppContentProvider
        public const string IncludeKey = "include";

        //ProjectSettingsProvider
        public const string ProjectSettingsKey = "__internalprojectsettings__";
        public const string ProjectVersionKey = "__internalprojectversion__";

        public const string ToolProjectVersionKey = "toolprojectversion";
        public const string ProjectIdKey = "__internalprojectid__";
        public const string TargetsKey = "targets";
        public const string ValidatedTargetsKey = "validatedtargets";
        public const string LibraryDescriptionKey = "__internallibrarydescription__";
        public const string LibraryVersionKey = "__internallibraryversion__";
        public const string EngineerVersionKey = "__internalengineerversion__";
        public const string SolutionVersionKey = "__internalsolutionversion__";
        public const string ProjectConfigurationsKey = "__internalprojectconfigurations__";
        public const string GenerateDTArrayNameByTypeKey = "generatedtarraynamebytype";
        public const string MinTargetVersionKey = "mintargetversion";
        public const string CSharpProjectPath = "csharpprojectpath";

        //DeployCommandContentProvider
        public const string InternalDeployPathKey = "__internaldeploypath__";
        public const string InternalConfigPathKey = "__internalconfigpath__";
        public const string ExcludeFilesKey = "excludefiles";

        //CMakeBuildContentProvider
        public const string BuildTypeKey = "buildtype";
        public const string InternalBuildSystemKey = "__internalbuildsystem__";
        public const string InternalBuildSystemDirectoryKey = "__internalbuildsystemdirectory__";
        public const string InternalExternalLibrariesKey = "__internalexternallibraries__";
        public const string InternalInstallationPathsKey = "__internalstagingprefix__";

        //CommandDefinitionContentProvider
        public const string GenerateDatatypeNamespaces = "generate-namespaces";

        //TargetParser
        public const string TargetFullNameKey = "targetfullname";
        public const string TargetShortFullNameKey = "targetshortfullname";
        public const string TargetEngineerVersionKey = "__internaltargetengineerversion__";
        public const string TargetVersionKey = "__internaltargetversion__";

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
