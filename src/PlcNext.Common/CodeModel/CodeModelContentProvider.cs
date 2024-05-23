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
using System.Text.RegularExpressions;
using PlcNext.Common.DataModel;
using PlcNext.Common.MetaData;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Format;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.CodeModel
{
    internal class CodeModelContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly IDatatypeConversion datatypeConversion;
        private readonly ExecutionContext executionContext;

        private static readonly Regex UnkownDataTypeRegex = new Regex(@"^unkown\((?<dataType>.*)\)$", RegexOptions.Compiled);
        private static readonly Regex UnknownDataTypeRegex = new Regex(@"^unknown\((?<dataType>.*)\)$", RegexOptions.Compiled);
        private static readonly Regex StaticStringRegex =
            new(@"^(?:(?:::)?Arp\S*::)?Static(W)?String<(?<length>\w*)>$", RegexOptions.Compiled);

        public CodeModelContentProvider(ITemplateRepository templateRepository, IDatatypeConversion datatypeConversion,
                                        ExecutionContext executionContext)
        {
            this.templateRepository = templateRepository;
            this.datatypeConversion = datatypeConversion;
            this.executionContext = executionContext;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<ICodeModel>() && key == EntityKeys.NamespaceKey ||
                   key == EntityKeys.FieldArpDataTypeKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.TypeMetaDataFormatKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.IecDataTypeFormatKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.IecDataTypeNamespaceKey ||
                   key == EntityKeys.ConvertToIECDatatypeKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.ExpandHiddenTypesFormatKey && owner.All(e => e.HasValue<IField>()) ||
                   key == EntityKeys.FilterHiddenTypesFormatKey && owner.All(e => e.HasValue<IType>()) ||
                   key == EntityKeys.IsFieldKey ||
                   key == EntityKeys.IsTypeKey ||
                   key == EntityKeys.BaseDirectoryKey && HasBaseDirectory(owner, out _);
        }

        private static bool HasBaseDirectory(Entity owner, out string baseDirectory)
        {
            baseDirectory = null;

            Entity current = owner;
            while (current != null)
            {
                if (current.HasValue<IType>())
                {
                    baseDirectory = current.Root.Value<ICodeModel>().GetBaseDirectory(current.Value<IType>()).FullName;
                    return true;
                }
                current = current.Owner;
            }

            return false;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            TemplateEntity ownerTemplateEntity = TemplateEntity.Decorate(owner);
            CodeEntity ownerCodeEntity = CodeEntity.Decorate(owner);
            if (key == EntityKeys.FieldArpDataTypeKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveArpDataType();
            }
            if (key == EntityKeys.TypeMetaDataFormatKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveTypeMetaDataFormat();
            }
            if (key == EntityKeys.IecDataTypeFormatKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveIECDataType();
            }
            if (key == EntityKeys.IecDataTypeNamespaceKey)
            {
                return ResolveDataTypeNamespace();
            }
            if (key == EntityKeys.ConvertToIECDatatypeKey && owner.Type == EntityKeys.FormatKey)
            {
                return ConvertToIECDatatype();
            }
            if (key == EntityKeys.ExpandHiddenTypesFormatKey)
            {
                return ExpandHiddenTypes();
            }
            if (key == EntityKeys.FilterHiddenTypesFormatKey)
            {
                return FilterHiddenTypes();
            }
            if (key == EntityKeys.IsFieldKey)
            {
                return owner.Create(key, CodeEntity.Decorate(owner).AsField != null);
            }
            if (key == EntityKeys.IsTypeKey)
            {
                return owner.Create(key, CodeEntity.Decorate(owner).AsType != null);
            }

            if (key == EntityKeys.BaseDirectoryKey && HasBaseDirectory(owner, out string baseDirectory))
            {
                return owner.Create(key, baseDirectory);
            }

            if (key == EntityKeys.NamespaceKey)
            {
                return GetNamespace();
            }

            Entity GetNamespace()
            {
                ICodeModel codeModel = owner.Value<ICodeModel>();
                bool prior206Target = CheckProjectTargets();
                IEnumerable<string> relevantTypes = TemplateEntity.Decorate(owner).EntityHierarchy
                              .Select(CodeEntity.Decorate)
                                                                  .Where(c => !c.IsRoot())
                                                                  .Select(c => c.FullName);
                string ns = prior206Target
                                ? codeModel.RootNamespaceForOldTarget(
                                    relevantTypes.ToArray(), ownerCodeEntity.PortStructs.Concat(ownerCodeEntity.PortEnums).Select(e => e.AsType)
                                       .Select(c => c.FullName)
                                       .ToArray())
                                : codeModel.RootNamespace(relevantTypes.ToArray());
                if (string.IsNullOrEmpty(ns))
                {
                    ns = owner.Name;
                }

                return owner.Create(key, ns);
            }

            throw new ContentProviderException(key, owner);

            bool CheckProjectTargets()
            {
                IEnumerable<TargetEntity> targets =
                    ProjectEntity.Decorate(owner).Targets.Select(t => TargetEntity.Decorate(t));
                return targets.Any(t => t.Version < new Version(20, 6));
            }

            Entity FilterHiddenTypes()
            {
                IEnumerable<IType> types = owner.Select(CodeEntity.Decorate)
                                                .Where(c => !c.IsHidden())
                                                .Select(c => c.AsType)
                                                .ToArray();
                string name = owner.FirstOrDefault()?.Name ?? string.Empty;
                return owner.Create(key, types.Select(t => owner.Create(name, t.FullName, t)));
            }

            Entity ExpandHiddenTypes()
            {
                IEnumerable<CodeEntity> dataSourceFields = owner.Select(CodeEntity.Decorate);
                dataSourceFields = ExpandHiddenStructureFields(dataSourceFields);
                string name = owner.FirstOrDefault()?.Name ?? string.Empty;

                return owner.Create(key, dataSourceFields.Select(e => e.AsField)
                                                         .Select(f => owner.Create(name, f.Name, f)));

                IEnumerable<CodeEntity> ExpandHiddenStructureFields(IEnumerable<CodeEntity> fields)
                {
                    foreach (CodeEntity hiddenField in fields)
                    {
                        CodeEntity hiddenType = hiddenField.ResolvedType;
                        if (hiddenType == null || !hiddenType.IsHidden())
                        {
                            yield return hiddenField;
                            continue;
                        }

                        foreach (CodeEntity hiddenTypeField in ExpandHiddenStructureFields(hiddenType.Fields))
                        {
                            yield return hiddenTypeField;
                        }
                    }
                }
            }

            Entity ResolveTypeMetaDataFormat()
            {
                IEntityBase dataSource = ownerTemplateEntity.FormatOrigin;
                IDataType dataSourceDataType = dataSource.HasValue<IDataType>()
                                                   ? dataSource.Value<IDataType>()
                                                   : dataSource.Value<IField>().DataType;

                ICodeModel rootCodeModel = dataSource.Root.Value<ICodeModel>();
                string dataTypeName = string.Empty;
                if (dataSourceDataType != null)
                {
                    dataTypeName = dataSourceDataType.PotentialFullNames
                                                     .Select(n => rootCodeModel.Type(n))
                                                     .FirstOrDefault(t => t != null)
                                                    ?.Name
                                   ?? datatypeConversion.Convert(dataSourceDataType);
                }

                IAttribute attribute = dataSource.HasValue<IField>()
                                        ? dataSource.Value<IField>()
                                            .Attributes?
                                            .FirstOrDefault(a => a.Name.Equals(EntityKeys.IECDataTypeAttributeNameKey, StringComparison.OrdinalIgnoreCase))
                                        : null;
                if (attribute != null)
                {
#pragma warning disable CA1308 // Normalize strings to uppercase
                    string attributeValue = attribute.Values.First().ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase

                    formatTemplate conversionTable = templateRepository.FormatTemplates
                                                                        .FirstOrDefault(t => t.name.Equals("CppToMetaTypeConversions",
                                                                                                           StringComparison.OrdinalIgnoreCase));
                    if (conversionTable?.Verify(dataTypeName, attributeValue) == true)
                    {
                        return owner.Create(key, attributeValue);
                        
                    }
                    ValidateIECDataTypeAttribute(dataTypeName, attributeValue);
                }

                return owner.Create(key, dataTypeName);
            }

            Entity ResolveArpDataType()
            {
                IEntityBase dataSource = ownerTemplateEntity.FormatOrigin;
                IField dataSourceField = dataSource.Value<IField>();
                if (dataSourceField == null)
                {
                    throw new FormatTargetMismatchException("arpDataType", "field|port", dataSource.Type);
                }

                ICodeModel rootCodeModel = dataSource.Root.Value<ICodeModel>();
                bool isStruct = dataSourceField.DataType.PotentialFullNames.Any(n => rootCodeModel.GetClass(n) != null || rootCodeModel.GetStructure(n) != null);
                IEnum @enum = dataSourceField.DataType.PotentialFullNames.Select(n => rootCodeModel.GetEnum(n)).FirstOrDefault(e => e != null);
                bool isArray = dataSourceField.Multiplicity.Any();
                string arpName = "DataType::" + GetArpDataType(dataSourceField.DataType.Name);

                return owner.Create(key, arpName);

                string GetArpDataType(string dataTypeName)
                {
                    string postfix = isArray ? " | DataType::Array" : string.Empty;

                    if (isStruct)
                    {
                        return "Struct" + postfix;
                    }

                    if (@enum != null)
                    {
                        (IDataType enumBaseType, string formattedBaseType) = GetEnumBaseType(@enum);

                        return $"Enum | DataType::{formattedBaseType}" + postfix;
                    }

                    (bool success, string value) = FormatDataType(dataTypeName);

                    if (!success)
                    {
                        throw new UnknownDataTypeException(value);
                    }

                    return value + postfix;
                }
            }

            (bool success, string value) FormatDataType(string unformattedValue)
            {
                if (unformattedValue.Contains('<', StringComparison.Ordinal))
                {
                    unformattedValue = unformattedValue.Substring(0, unformattedValue.IndexOf('<', StringComparison.Ordinal));
                }

                string result = owner.Create("temporaryCtnFormatContainer", unformattedValue).Format()["knownDataTypes"].Value<string>();
                Match unkownMatch = UnkownDataTypeRegex.Match(result);

                if (unkownMatch.Success)
                {
                    return (false, unkownMatch.Groups["dataType"].Value);
                }

                return (true, result);
            }

            (bool success, string value) FormatIecDataType(string unformattedValue, IAttribute attribute)
            {

                string formattedString = FormatStringDataType(unformattedValue);
                if (formattedString != null)
                {
                    if (attribute != null)
                    {
                        executionContext.WriteWarning($"The //#{EntityKeys.IECDataTypeAttributeNameKey}(...) can not be set for string datatypes and will be ignored.");
                    }
                    return (true, formattedString);
                }

                (bool success, string value) = FormatDataType(unformattedValue);

                if (!success)
                {
                    return (success, value);
                }


                if (attribute != null)
                {
                    string attributeValue = attribute.Values.First();

                    ValidateIECDataTypeAttribute(value, attributeValue);

                    return (true, attributeValue.ToUpperInvariant());
                }
                else
                {
                    string result = owner.Create("temporaryIecFormatContainer", value).Format()["iecDataType"].Value<string>();
                    Match unknownMatch = UnknownDataTypeRegex.Match(result);

                    if (unknownMatch.Success)
                    {
                        return (false, unknownMatch.Groups["dataType"].Value);
                    }

                    return (true, result);
                }
            }

            void ValidateIECDataTypeAttribute(string raw, string converted)
            {
                formatTemplate conversionTable = templateRepository.FormatTemplates
                                                                    .FirstOrDefault(t => t.name.Equals("AllowedcpptoIECDataTypeConversions",
                                                                                                       StringComparison.OrdinalIgnoreCase));
                if (conversionTable?.Verify(raw, converted) == false)
                {
                    throw new IECAttributeMismatchException(raw, converted);
                }
            }
            
            string FormatStringDataType(string unformattedValue)
            {
                Match stringMatch = StaticStringRegex.Match(unformattedValue);
                if (stringMatch.Success)
                {
                    return owner.Create("temporaryStaticStringFormatContainer", unformattedValue)
                                .Format()["stringConstantReplace"]
                                .Format()["iecStringDataType"].Value<string>();
                }
                return null;
            }

            Entity ResolveIECDataType()
            {
                IEntityBase dataSource = ownerTemplateEntity.FormatOrigin;
                IDataType dataSourceDataType;
                IAttribute attribute;
                if (dataSource.HasValue<IEnum>())
                {
                    IEnum dataSourceEnum = dataSource.Value<IEnum>();
                    dataSourceDataType = dataSourceEnum.BaseTypes.FirstOrDefault();
                    attribute = dataSourceEnum.Attributes?
                                              .FirstOrDefault(a => a.Name.Equals(EntityKeys.IECDataTypeAttributeNameKey,
                                                                                 StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    dataSourceDataType = dataSource.HasValue<IDataType>()
                                                       ? dataSource.Value<IDataType>()
                                                       : dataSource.Value<IField>().DataType;

                    attribute = dataSource.HasValue<IField>()
                                ? dataSource.Value<IField>()
                                            .Attributes?
                                            .FirstOrDefault(a => a.Name.Equals(EntityKeys.IECDataTypeAttributeNameKey, StringComparison.OrdinalIgnoreCase))
                                : null;
                }

                (bool success, string dataTypeName) = FormatIecDataType(dataSourceDataType.Name, attribute);

                if (!success)
                {
                    ICodeModel rootCodeModel = dataSource.Root.Value<ICodeModel>();
                    IType knownType = dataSourceDataType.PotentialFullNames
                                      .Select(n => rootCodeModel.Type(n))
                                      .FirstOrDefault(t => t != null);
                    if (knownType == null)
                    {
                        throw new UnknownIecDataTypeException(dataTypeName);
                    }
                    dataTypeName = dataSource.Create("temporaryTypeFormatContainer", knownType.Name)
                                        .Format()["escapeTemplatedStructName"]
                                        .Format()["convertToIECDataType"].Value<string>();
                }
                dataTypeName = dataTypeName.Contains("::", StringComparison.Ordinal)
                    ? dataTypeName.Substring(dataTypeName.LastIndexOf("::", StringComparison.InvariantCulture) + "::".Length)
                    : dataTypeName;

                return owner.Create(key, dataTypeName);
            }

            Entity ResolveDataTypeNamespace()
            {
                IEntityBase dataSource = ownerTemplateEntity.FormatOrigin;
                IDataType dataSourceDataType;
                IAttribute attribute;
                if (dataSource.HasValue<IEnum>())
                {
                    IEnum dataSourceEnum = dataSource.Value<IEnum>();
                    dataSourceDataType = dataSourceEnum.BaseTypes.FirstOrDefault();
                    attribute = dataSourceEnum.Attributes?
                                              .FirstOrDefault(a => a.Name.Equals(EntityKeys.IECDataTypeAttributeNameKey,
                                                                                 StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    dataSourceDataType = dataSource.HasValue<IDataType>()
                                                       ? dataSource.Value<IDataType>()
                                                       : dataSource.Value<IField>().DataType;

                    attribute = dataSource.HasValue<IField>()
                                ? dataSource.Value<IField>()
                                            .Attributes?
                                            .FirstOrDefault(a => a.Name.Equals(EntityKeys.IECDataTypeAttributeNameKey, StringComparison.OrdinalIgnoreCase))
                                : null;
                }

                (bool success, _) = FormatIecDataType(dataSourceDataType.Name, attribute);

                if (!success)
                {
                    ICodeModel rootCodeModel = dataSource.Root.Value<ICodeModel>();
                    IType knownType = dataSourceDataType.PotentialFullNames
                                      .Select(n => rootCodeModel.Type(n))
                                      .FirstOrDefault(t => t != null);
                    if (knownType == null)
                    {
                        throw new UnknownIecDataTypeException(dataSourceDataType.Name);
                    }

                    return owner.Create(key, string.IsNullOrEmpty(knownType.Namespace) ? knownType.Namespace : $"{knownType.Namespace}.");
                }
                return owner.Create(key, string.Empty);
            }

            (IDataType, string) GetEnumBaseType(IEnum @enum)
            {
                IDataType enumDataType = @enum.BaseTypes.FirstOrDefault();
                string enumBaseType = enumDataType?.Name;
                if (string.IsNullOrEmpty(enumBaseType))
                {
                    throw new MissingEnumDataTypeException(@enum);
                }

                (bool formatted, string formattedBaseType) = FormatDataType(enumBaseType);
                if (!formatted)
                {
                    throw new UnkownBaseDataTypeException(formattedBaseType, @enum);
                }

                return (enumDataType, formattedBaseType);
            }

            Entity ConvertToIECDatatype()
            {
                Entity dataSource = ownerTemplateEntity.Owner;
                string result = dataSource.Value<string>();
                string[] parts = result.Split('_');
                var x = parts.Select(p => UnknownDataTypeRegex.IsMatch(owner.Create("temporaryFormatContainer", p)
                                                .Format()["knownDataTypes"]
                                                .Format()["iecDataType"]
                                                .Value<string>())
                                            ?p
                                            : owner.Create("temporaryFormatContainer", p)
                                                .Format()["knownDataTypes"]
                                                .Format()["iecDataType"]
                                                .Value<string>());
                result = string.Join("_", x);
                return owner.Create(key, result);
            }
        }
    }
}
