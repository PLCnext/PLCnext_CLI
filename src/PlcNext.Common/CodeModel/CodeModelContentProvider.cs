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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.DataModel;
using PlcNext.Common.MetaData;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using multiplicity = PlcNext.Common.Templates.Field.multiplicity;

namespace PlcNext.Common.CodeModel
{
    internal class CodeModelContentProvider : IEntityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateResolver resolver;
        private readonly IDatatypeConversion datatypeConversion;

        private static readonly Regex UnkownDataTypeRegex = new Regex(@"^unkown\((?<dataType>.*)\)$", RegexOptions.Compiled);

        public CodeModelContentProvider(ITemplateRepository templateRepository, ITemplateResolver resolver, IDatatypeConversion datatypeConversion)
        {
            this.templateRepository = templateRepository;
            this.resolver = resolver;
            this.datatypeConversion = datatypeConversion;
        }

        public bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<ICodeModel>() &&
                   (key == EntityKeys.NamespaceKey ||
                    key == EntityKeys.PortStructsKey ||
                    key == EntityKeys.PortEnumsKey) ||
                   owner.HasValue<IType>() && CanResolveType() ||
                   owner.HasValue<IField>() && CanResolveField() ||
                   owner.HasValue<IDataType>() && CanResolveDataType() ||
                   owner.HasValue<ISymbol>() && CanResolveSymbol() ||
                   key == EntityKeys.FieldArpDataTypeKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.TypeMetaDataFormatKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.BaseDirectoryKey && HasBaseDirectory(owner, out _);

            bool CanResolveSymbol()
            {
                return owner.Value<ISymbol>().HasPropertyValueEntity(key);
            }

            bool CanResolveDataType()
            {
                return key == EntityKeys.NameKey ||
                       key == EntityKeys.FullNameKey;
            }

            bool CanResolveField()
            {
                fieldTemplate fieldTemplate = templateRepository.FieldTemplates
                                                                .FirstOrDefault(t => t.name.Equals(key,
                                                                                                   StringComparison.OrdinalIgnoreCase));
                return key == EntityKeys.FieldNameKey ||
                       key == EntityKeys.DataTypeKey ||
                       key == EntityKeys.MultiplicityKey ||
                       fieldTemplate != null;
            }

            bool CanResolveType()
            {
                if (key == EntityKeys.TemplateKey)
                {
                    return false;
                }
                IType type = owner.Value<IType>();
                
                fieldTemplate fieldTemplate = templateRepository.FieldTemplates
                                                                .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                                                            StringComparison
                                                                                                               .OrdinalIgnoreCase));

                templateRelationship[] relationships = owner.HasTemplate() ? owner.Template().Relationship : null;
                templateRelationship relationship = relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                                                    ?? relationships?.FirstOrDefault(r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));

                return key == EntityKeys.PathKey ||
                       key == EntityKeys.FieldsKey ||
                       key == EntityKeys.BaseTypeKey ||
                       key == EntityKeys.FileKey ||
                       type.HasPropertyValueEntity(key) ||
                       fieldTemplate != null ||
                       relationship != null && type.Attributes.Any(a => a.Name.Equals(relationship.name,
                                                                       StringComparison.OrdinalIgnoreCase));
            }
        }

        private bool HasBaseDirectory(Entity owner, out string baseDirectory)
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

        public Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            TemplateEntity ownerTemplateEntity = TemplateEntity.Decorate(owner);

            if (key == EntityKeys.FieldArpDataTypeKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveArpDataType();
            }
            if (key == EntityKeys.TypeMetaDataFormatKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveTypeMetaDataFormat();
            }

            ISymbol symbol = owner.Value<ISymbol>();
            if (symbol != null)
            {
                Entity result = owner.PropertyValueEntity(key, symbol);
                if (result != null)
                {
                    return result;
                }

                throw new ContentProviderException(key, owner);
            }

            if (key == EntityKeys.BaseDirectoryKey && HasBaseDirectory(owner, out string baseDirectory))
            {
                return owner.Create(key, baseDirectory);
            }

            IType type = owner.Value<IType>();
            if (type != null)
            {
                return ResolveType();
            }

            IField field = owner.Value<IField>();
            if (field != null)
            {
                return ResolveField();
            }

            IDataType dataType = owner.Value<IDataType>();
            if (dataType != null)
            {
                return ResolveDataType();
            }

            ICodeModel codeModel = owner.Value<ICodeModel>();
            if (codeModel != null)
            {
                return ResolveCodeModel();
            }
            throw new ContentProviderException(key, owner);

            IEnumerable<IType> GetPortEnums()
            {
                return GetAllPorts().Concat(GetPortStructures().SelectMany(p => p.Fields))
                                    .SelectMany(f => f.DataType.PotentialFullNames)
                                    .Select(codeModel.Enum)
                                    .Where(e => e != null)
                                    .Distinct();
            }

            IEnumerable<IField> GetAllPorts()
            {
                bool IsPort(IField arg)
                {
                    return arg.HasAttributeWithoutValue(EntityKeys.PortAttributeKey);
                }

                return codeModel.Types.Keys
                                .SelectMany(t => t.Fields.Where(IsPort));
            }

            IEnumerable<IType> GetPortStructures()
            {
                HashSet<IType> structures = new HashSet<IType>(GetAllPorts()
                                                              .SelectMany(f => f.DataType.PotentialFullNames)
                                                              .Select(codeModel.Type)
                                                              .Where(s => s != null));

                HashSet<IType> visited = new HashSet<IType>();
                while (structures.Except(visited).Any())
                {
                    foreach (IType structure in structures.Except(visited).ToArray())
                    {
                        foreach (IField structureField in structure.Fields)
                        {
                            IType structureDataType = structureField.DataType.PotentialFullNames
                                                           .Select(codeModel.Type)
                                                           .FirstOrDefault(s => s != null);
                            if (structureDataType != null)
                            {
                                structures.Add(structureDataType);
                            }
                        }

                        visited.Add(structure);
                    }
                }

                return structures;
            }

            Entity ResolveType()
            {
                switch (key)
                {
                    case EntityKeys.PathKey:
                    {
                        return owner.Create(key, type.GetFile(owner).Parent.FullName);
                    }
                    case EntityKeys.FieldsKey:
                    {
                        return owner.Create(key, type.Fields.Select(f => owner.Create(key.Singular(), f.Name, f)));
                    }
                    case EntityKeys.FileKey:
                    {
                        VirtualFile file = owner.Root.Value<ICodeModel>()
                                                .Types[type];
                        return owner.Create(key, file.Name, file);
                    }
                    case EntityKeys.BaseTypeKey:
                    {
                        IDataType baseType = type.BaseTypes.FirstOrDefault();
                        return owner.Create(key, baseType);
                    }
                    default:
                    {
                        Entity result = owner.PropertyValueEntity(key, type);
                        if (result != null)
                        {
                            return result;
                        }
                        
                        fieldTemplate fieldTemplate = templateRepository.FieldTemplates
                                                                        .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                                                                 StringComparison
                                                                                                                    .OrdinalIgnoreCase));
                        if (fieldTemplate != null)
                        {
                            IEnumerable<IField> fields = type.Fields
                                                             .Where(f => f.Attributes
                                                                          .Any(a => a.Name
                                                                                     .Equals(fieldTemplate.name,
                                                                                             StringComparison.OrdinalIgnoreCase)));
                            return owner.Create(key, fields.Select(f => owner.Create(fieldTemplate.name, f.Name, f)));
                        }

                        templateRelationship[] relationships = owner.Template().Relationship;
                        templateRelationship relationship = relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                                                            ?? relationships?.FirstOrDefault(r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));
                        TemplateDescription relationshipDescription = relationship != null ? templateRepository.Template(relationship.type) : null;
                        if (relationshipDescription != null)
                        {
                            IEnumerable<string> names = type.Attributes
                                                            .Where(a => a.Name.Equals(relationship.name,
                                                                                      StringComparison.OrdinalIgnoreCase))
                                                            .SelectMany(a => a.Values);
                            return relationship.GetRelationship(relationshipDescription, owner, names.ToArray());
                        }

                        throw new ContentProviderException(key, owner);
                    }
                }
            }

            Entity ResolveField()
            {
                switch (key)
                {
                    case EntityKeys.FieldNameKey:
                    {
                        return owner.Create(key, field.Name);
                    }
                    case EntityKeys.DataTypeKey:
                    {
                        IDataType fieldDataType = GetRealFieldDataType();
                        return owner.Create(key, fieldDataType.Name, fieldDataType);
                    }
                    case EntityKeys.MultiplicityKey:
                    {
                        return owner.Create(key, field.Multiplicity.Select(m => owner.Create(key, new Func<string>(m.ToString), m)));
                    }
                    default:
                    {
                        fieldTemplate fieldTemplate = templateRepository.FieldTemplates
                                                                        .FirstOrDefault(t => t.name.Equals(key,
                                                                                                           StringComparison.OrdinalIgnoreCase));
                        if (fieldTemplate != null)
                        {
                            return GetFieldTemplateEntity(fieldTemplate);
                        }
                        throw new ContentProviderException(key, owner);
                    }
                }

                IDataType GetRealFieldDataType()
                {
                    ICodeModel rootCodeModel = owner.Root.Value<ICodeModel>();
                    IEnum @enum = field.DataType.PotentialFullNames.Select(n => rootCodeModel.Enum(n)).FirstOrDefault(e => e != null);
                    if (@enum != null)
                    {
                        (IDataType enumBaseType, _) = GetEnumBaseType(@enum);
                        return enumBaseType;
                    }

                    return field.DataType;
                }

                Entity GetFieldTemplateEntity(fieldTemplate fieldTemplate)
                {
                    if (!fieldTemplate.hasvalue)
                    {
                        bool hasAttribute = field.HasAttributeWithoutValue(fieldTemplate.name);
                        return owner.Create(key, new Func<string>(hasAttribute.ToString), hasAttribute);
                    }
                    (string[] values, CodePosition position) = GetFieldTemplateValue();
                    IEnumerable<string> formattedValues = VerifyValues().ToArray();

                    return fieldTemplate.multiplicity == multiplicity.OneOrMore
                               ? owner.Create(key, formattedValues.Select(v => owner.Create(key.Singular(), v)))
                               : owner.Create(key, formattedValues.First());

                    IEnumerable<string> VerifyValues()
                    {
                        return values.Select(VerifyValue);

                        string VerifyValue(string arg)
                        {
                            (bool success, string message, string newValue) = fieldTemplate.ValueRestriction.Verify(arg);
                            if (!success)
                            {
                                owner.AddCodeException(new FieldAttributeRestrictionException(fieldTemplate.name, arg, message, position, field.GetFile(owner)));
                            }

                            return newValue;
                        }
                    }

                    (string[], CodePosition) GetFieldTemplateValue()
                    {
                        IAttribute attribute = field.Attributes.LastOrDefault(a => a.Name.Equals(fieldTemplate.name,
                                                                                                  StringComparison.OrdinalIgnoreCase));
                        string value = attribute?.Values.FirstOrDefault() ??
                                       resolver.Resolve(fieldTemplate.defaultvalue, owner);
                        return (fieldTemplate.multiplicity == multiplicity.OneOrMore
                                    ? value.Split(new[] {fieldTemplate.split}, StringSplitOptions.RemoveEmptyEntries)
                                    : new[] {value},
                                attribute?.Position ?? new CodePosition(0, 0));
                    }
                }
            }

            Entity ResolveCodeModel()
            {
                switch (key)
                {
                    case EntityKeys.PortStructsKey:
                    {
                        return owner.Create(key, GetPortStructures().Select(t => owner.Create("type", t.FullName, t)));
                    }
                    case EntityKeys.PortEnumsKey:
                    {
                        return owner.Create(key, GetPortEnums().Select(t => owner.Create("type", t.FullName, t)));
                    }
                    case EntityKeys.NamespaceKey:
                    {
                        string ns = codeModel.RootNamespace(templateRepository.Templates);
                        if (string.IsNullOrEmpty(ns))
                        {
                            ns = owner.Name;
                        }

                        return owner.Create(key, ns);
                    }
                    default:
                        throw new ContentProviderException(key, owner);
                }
            }

            Entity ResolveDataType()
            {
                switch (key)
                {
                    case EntityKeys.NameKey:
                        return owner.Create(key, dataType.Name);
                    case EntityKeys.FullNameKey:
                        string fullName = GetDataTypeFullName();
                        return owner.Create(key, fullName);
                    default:
                        throw new ContentProviderException(key, owner);
                }

                string GetDataTypeFullName()
                {
                    ICodeModel rootCodeModel = owner.Root.Value<ICodeModel>();
                    return dataType.PotentialFullNames.FirstOrDefault(fn => rootCodeModel.Type(fn) != null)
                           ?? dataType.Name;
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
                bool isStruct = dataSourceField.DataType.PotentialFullNames.Any(n => rootCodeModel.Type(n) != null);
                IEnum @enum = dataSourceField.DataType.PotentialFullNames.Select(n => rootCodeModel.Enum(n)).FirstOrDefault(e => e != null);
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
                if (unformattedValue.Contains('<'))
                {
                    unformattedValue = unformattedValue.Substring(0, unformattedValue.IndexOf('<'));
                }

                string result = owner.Create("temporaryCtnFormatContainer", unformattedValue).Format()["knownDataTypes"].Value<string>();
                Match unkownMatch = UnkownDataTypeRegex.Match(result);

                if (unkownMatch.Success)
                {
                    return (false, unkownMatch.Groups["dataType"].Value);
                }

                return (true, result);
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
                    throw new UnkownEnumDataTypeException(formattedBaseType, @enum);
                }

                return (enumDataType, formattedBaseType);
            }
        }
    }
}
