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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PlcNext.Common.DataModel;
using PlcNext.Common.MetaData;
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Description;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Priority;
using multiplicity = PlcNext.Common.Templates.Field.multiplicity;

namespace PlcNext.Common.CodeModel
{
    internal class CodeModelContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateResolver resolver;
        private readonly IDatatypeConversion datatypeConversion;

        private static readonly Regex UnkownDataTypeRegex = new Regex(@"^unkown\((?<dataType>.*)\)$", RegexOptions.Compiled);
        private static readonly Regex UnknownDataTypeRegex = new Regex(@"^unknown\((?<dataType>.*)\)$", RegexOptions.Compiled);

        private static readonly Regex StaticStringRegex = new Regex(@"^Static(W)?String<(?<length>\d+)>$", RegexOptions.Compiled);

        public CodeModelContentProvider(ITemplateRepository templateRepository, ITemplateResolver resolver, IDatatypeConversion datatypeConversion)
        {
            this.templateRepository = templateRepository;
            this.resolver = resolver;
            this.datatypeConversion = datatypeConversion;
        }

        public override SubjectIdentifier LowerPrioritySubject => nameof(ConstantContentProvider);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<ICodeModel>() &&
                   (key == EntityKeys.NamespaceKey ||
                    key == EntityKeys.PortStructsKey ||
                    key == EntityKeys.PortEnumsKey ||
                    key == EntityKeys.PortArraysKey ||
                    key == EntityKeys.VariablePortStringsKey) ||
                   owner.HasValue<IType>() && CanResolveType() ||
                   owner.HasValue<IField>() && CanResolveField() ||
                   owner.HasValue<IDataType>() && CanResolveDataType() ||
                   owner.HasValue<ISymbol>() && CanResolveSymbol() ||
                   key == EntityKeys.FieldArpDataTypeKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.TypeMetaDataFormatKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.IecDataTypeFormatKey && owner.Type == EntityKeys.FormatKey ||
                   key == EntityKeys.ExpandHiddenTypesFormatKey && owner.All(e => e.HasValue<IField>()) ||
                   key == EntityKeys.FilterHiddenTypesFormatKey && owner.All(e => e.HasValue<IType>()) ||
                   key == EntityKeys.IsFieldKey ||
                   key == EntityKeys.IsTypeKey ||
                   key == EntityKeys.BaseDirectoryKey && HasBaseDirectory(owner, out _) ||
                   key == EntityKeys.BigDataProjectKey ||
                   key == EntityKeys.NormalProjectKey ;

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
                metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                .FirstOrDefault(t => t.name.Equals(key,
                                                                                                   StringComparison.OrdinalIgnoreCase));
                return key == EntityKeys.FieldNameKey ||
                       key == EntityKeys.DataTypeKey ||
                       key == EntityKeys.MultiplicityKey ||
                       key == EntityKeys.ResolvedTypeKey ||
                       key == EntityKeys.IsArray ||
                       metaDataTemplate != null;
            }

            bool CanResolveType()
            {
                if (key == EntityKeys.TemplateKey)
                {
                    return false;
                }
                IType type = owner.Value<IType>();
                
                metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                                                            StringComparison
                                                                                                               .OrdinalIgnoreCase));

                Templates.Type.metaDataTemplate typeMetaDataTemplate = templateRepository.TypeTemplates
                                                                      .FirstOrDefault(t => t.name.Equals(key, StringComparison.OrdinalIgnoreCase));

                templateRelationship[] relationships = owner.HasTemplate() ? owner.Template().Relationship : null;
                templateRelationship relationship = relationships?.FirstOrDefault(r => r.name.Equals(key, StringComparison.OrdinalIgnoreCase))
                                                    ?? relationships?.FirstOrDefault(r => r.name.Equals(key.Singular(), StringComparison.OrdinalIgnoreCase));

                return key == EntityKeys.PathKey ||
                       key == EntityKeys.FieldsKey ||
                       key == EntityKeys.BaseTypeKey ||
                       key == EntityKeys.FileKey ||
                       type.HasPropertyValueEntity(key) ||
                       metaDataTemplate != null ||
                       typeMetaDataTemplate != null ||
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

        private class FullNameCodeEntityComparer : IEqualityComparer<CodeEntity>
        {
            public bool Equals(CodeEntity x, CodeEntity y)
            {
                return $"{x.Namespace}.{x.Name}" == $"{y.Namespace}.{y.Name}";
            }

            public int GetHashCode(CodeEntity obj)
            {
                return $"{obj.Namespace}.{obj.Name}".GetHashCode();
            }
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
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
            if(key == EntityKeys.IecDataTypeFormatKey && owner.Type == EntityKeys.FormatKey)
            {
                return ResolveIECDataType();
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
            if (key == EntityKeys.BigDataProjectKey)
            {
                return GetBigDataProjectEntity();
            }
            if (key == EntityKeys.NormalProjectKey)
            {
                return GetNormalProjectEntity();
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

            IEnumerable<CodeEntity> GetPortEnums()
            {
                return GetAllPorts().Concat(GetPortStructures().SelectMany(p => p.Fields))
                                    .Select(f => f.ResolvedType)
                                    .Where(t => t.AsEnum != null)
                                    .Distinct(new FullNameCodeEntityComparer());
            }

            IEnumerable<CodeEntity> GetPortArrays()
            {
                return GetAllPorts().Concat(GetPortStructures().SelectMany(p => p.Fields))
                                    .Where(t => t.AsField != null &&
                                                t.AsField.Multiplicity.Count > 0)
                                    .Distinct();
            }

            IEnumerable<CodeEntity> GetVariablePortStrings()
            {
                return GetAllPorts().Where(t => t.AsField != null &&
                                                StaticStringRegex.IsMatch(t.AsField.DataType.Name) &&
                                                StaticStringRegex.Match(t.AsField.DataType.Name).Groups["length"].Value != "80" &&
                                                t.AsField.Multiplicity.Count == 0)
                                    .Distinct();
            }

            IEnumerable<CodeEntity> GetAllPorts()
            {
                bool IsPort(CodeEntity fieldEntity)
                {
                    return fieldEntity.AsField != null &&
                           fieldEntity.AsField.HasAttributeWithoutValue(EntityKeys.PortAttributeKey);
                }

                return TemplateEntity.Decorate(owner).EntityHierarchy
                              .Select(CodeEntity.Decorate)
                              .SelectMany(c => c.Fields)
                              .Where(IsPort);
            }

            IEnumerable<CodeEntity> GetPortStructures()
            {
                HashSet<CodeEntity> structures = new HashSet<CodeEntity>(GetAllPorts()
                                                              .Select(f => f.ResolvedType)
                                                              .Where(t => t.AsType != null && 
                                                                          t.AsEnum == null),
                    new FullNameCodeEntityComparer());

                HashSet<CodeEntity> visited = new HashSet<CodeEntity>(new FullNameCodeEntityComparer());
                while (structures.Except(visited).Any())
                {
                    foreach (CodeEntity structure in structures.Except(visited).ToArray())
                    {
                        foreach (CodeEntity structureField in structure.Fields)
                        {
                            CodeEntity structureDataType = structureField.ResolvedType;
                            if (structureDataType.AsType != null && structureDataType.AsEnum == null)
                            {
                                structures.Add(structureDataType);
                            }
                        }

                        visited.Add(structure);
                    }
                }

                return structures;
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

            T ChooseMetaDataTemplate<T>(IEnumerable<(T template, string name, string context)> templates)
            {
                return templates.OrderByDescending(t => ContextComplexity(t.context??string.Empty))
                                .FirstOrDefault(Matches)
                                .template;

                bool Matches((T template, string name, string context) template)
                {
                    if (!key.Equals(template.name, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    if (string.IsNullOrEmpty(template.context))
                    {
                        return true;
                    }
                    string[] contextPath = template.context.Split(new []{'/','\\'}, StringSplitOptions.RemoveEmptyEntries)
                                                   .Reverse().ToArray();
                    Entity current = owner;
                    foreach (string part in contextPath)
                    {
                        current = current?.Owner;
                        if (current?[$"is{part}"].Value<bool>() != true)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                int ContextComplexity(string context)
                {
                    return context.Count(c => c == '/' || c == '\\');
                }
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
                        if (TryResolveTypeTemplate(out Entity result))
                        {
                            return result;
                        }

                        if (TryResolveProperty(out result))
                        {
                            return result;
                        }

                        if (TryResolveFields(out result))
                        {
                            return result;
                        }

                        if (TryResolveRelationship(out result))
                        {
                            return result;
                        }

                        throw new ContentProviderException(key, owner);
                    }
                }

                bool TryResolveTypeTemplate(out Entity result)
                {
                    Templates.Type.metaDataTemplate typeMetaDataTemplate = ChooseMetaDataTemplate(templateRepository.TypeTemplates
                                                                                                                    .Select(t => (t,t.name,t.context)));
                    if (typeMetaDataTemplate != null)
                    {
                        result = GetTypeTemplateEntity(typeMetaDataTemplate);
                        return true;
                    }

                    result = null;
                    return false;

                    Entity GetTypeTemplateEntity(Templates.Type.metaDataTemplate metaDataTemplate)
                    {
                        if (!metaDataTemplate.hasvalue)
                        {
                            bool hasAttribute = type.HasAttributeWithoutValue(metaDataTemplate.name);
                            return owner.Create(key, new Func<string>(hasAttribute.ToString), hasAttribute);
                        }
                        (string[] values, CodePosition position) = GetTypeTemplateValue();
                        IEnumerable<string> formattedValues = VerifyValues().ToArray();

                        return metaDataTemplate.multiplicity == Templates.Type.multiplicity.OneOrMore
                                   ? owner.Create(key, formattedValues.Select(v => owner.Create(key.Singular(), v)))
                                   : owner.Create(key, formattedValues.First());

                        IEnumerable<string> VerifyValues()
                        {
                            return values.Select(VerifyValue);

                            string VerifyValue(string arg)
                            {
                                (bool success, string message, string newValue) = metaDataTemplate.ValueRestriction.Verify(arg);
                                if (!success)
                                {
                                    owner.AddCodeException(new FieldAttributeRestrictionException(metaDataTemplate.name, arg, message, position, type.GetFile(owner)));
                                }

                                return newValue;
                            }
                        }

                        (string[], CodePosition) GetTypeTemplateValue()
                        {
                            IAttribute attribute = type.Attributes.LastOrDefault(a => a.Name.Equals(metaDataTemplate.name,
                                                                                                      StringComparison.OrdinalIgnoreCase));
                            string value = attribute?.Values.FirstOrDefault() ??
                                           resolver.Resolve(metaDataTemplate.defaultvalue, owner);
                            return (metaDataTemplate.multiplicity == Templates.Type.multiplicity.OneOrMore
                                        ? value.Split(new[] { metaDataTemplate.split }, StringSplitOptions.RemoveEmptyEntries)
                                        : new[] { value },
                                    attribute?.Position ?? new CodePosition(0, 0));
                        }
                    }
                }

                bool TryResolveProperty(out Entity result)
                {
                    result = owner.PropertyValueEntity(key, type);
                    return result != null;
                }

                bool TryResolveFields(out Entity result)
                {
                    metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                    .FirstOrDefault(t => t.name.Plural().Equals(key,
                                                                                                                StringComparison
                                                                                                                   .OrdinalIgnoreCase));
                    if (metaDataTemplate != null)
                    {
                        IEnumerable<IField> fields = type.Fields
                                                         .Where(f => f.Attributes
                                                                      .Any(a => a.Name
                                                                                 .Equals(metaDataTemplate.name,
                                                                                         StringComparison.OrdinalIgnoreCase)));
                        result = owner.Create(key, fields.Select(f => owner.Create(metaDataTemplate.name, f.Name, f)));
                        return true;
                    }

                    result = null;
                    return false;

                    
                }

                bool TryResolveRelationship(out Entity result)
                {
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
                        result = relationship.GetRelationship(relationshipDescription, owner, names.ToArray());
                        return true;
                    }

                    result = null;
                    return false;
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
                    case EntityKeys.ResolvedTypeKey:
                    {
                        ICodeModel model = owner.Root.Value<ICodeModel>();
                        IDataType fieldDataType = field.DataType;
                        IType realType = fieldDataType.PotentialFullNames
                                                      .Select(model.Type)
                                                      .FirstOrDefault(t => t != null);
                        return owner.Create(key, realType?.Name, realType);
                    }
                    case EntityKeys.MultiplicityKey:
                    {
                        return owner.Create(key, field.Multiplicity.Select(m => owner.Create(key, new Func<string>(m.ToString), m)));
                    }
                    case EntityKeys.IsArray:
                        if (field.Multiplicity.Any())
                            return owner.Create(key, true.ToString(CultureInfo.InvariantCulture), true);
                        return owner.Create(key, false.ToString(CultureInfo.InvariantCulture), false);
                    default:
                    {
                        metaDataTemplate metaDataTemplate = templateRepository.FieldTemplates
                                                                        .FirstOrDefault(t => t.name.Equals(key,
                                                                                                           StringComparison.OrdinalIgnoreCase));
                        if (metaDataTemplate != null)
                        {
                            return GetFieldTemplateEntity(metaDataTemplate);
                        }
                        throw new ContentProviderException(key, owner);
                    }
                }

                IDataType GetRealFieldDataType()
                {
                    ICodeModel rootCodeModel = owner.Root.Value<ICodeModel>();
                    IEnum @enum = field.DataType.PotentialFullNames.Select(n => rootCodeModel.GetEnum(n)).FirstOrDefault(e => e != null);
                    if (@enum != null)
                    {
                        (IDataType enumBaseType, _) = GetEnumBaseType(@enum);
                        return enumBaseType;
                    }

                    return field.DataType;
                }

                Entity GetFieldTemplateEntity(metaDataTemplate metaDataTemplate)
                {
                    if (!metaDataTemplate.hasvalue)
                    {
                        bool hasAttribute = field.HasAttributeWithoutValue(metaDataTemplate.name);
                        return owner.Create(key, new Func<string>(hasAttribute.ToString), hasAttribute);
                    }
                    (string[] values, CodePosition position) = GetFieldTemplateValue();
                    IEnumerable<string> formattedValues = VerifyValues().ToArray();

                    return metaDataTemplate.multiplicity == multiplicity.OneOrMore
                               ? owner.Create(key, formattedValues.Select(v => owner.Create(key.Singular(), v)))
                               : owner.Create(key, formattedValues.First());

                    IEnumerable<string> VerifyValues()
                    {
                        return values.Select(VerifyValue);

                        string VerifyValue(string arg)
                        {
                            (bool success, string message, string newValue) = metaDataTemplate.ValueRestriction.Verify(arg);
                            if (!success)
                            {
                                owner.AddCodeException(new FieldAttributeRestrictionException(metaDataTemplate.name, arg, message, position, field.GetFile(owner)));
                            }

                            return newValue;
                        }
                    }

                    (string[], CodePosition) GetFieldTemplateValue()
                    {
                        IAttribute attribute = field.Attributes.LastOrDefault(a => a.Name.Equals(metaDataTemplate.name,
                                                                                                  StringComparison.OrdinalIgnoreCase));
                        string value = string.IsNullOrEmpty(attribute?.Values.FirstOrDefault())
                                           ? resolver.Resolve(metaDataTemplate.defaultvalue, owner)
                                           : attribute.Values.First();
                        return (metaDataTemplate.multiplicity == multiplicity.OneOrMore
                                    ? value.Split(new[] {metaDataTemplate.split}, StringSplitOptions.RemoveEmptyEntries)
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
                        return owner.Create(key, GetPortStructures().Select(c => c.Base));
                    }
                    case EntityKeys.PortEnumsKey:
                    {
                        return owner.Create(key, GetPortEnums().Select(c => c.Base));
                    }
                    case EntityKeys.PortArraysKey:
                    {
                        return owner.Create(key, GetPortArrays().Select(c => c.Base));
                    }
                    case EntityKeys.VariablePortStringsKey:
                    {
                        return owner.Create(key, GetVariablePortStrings().Select(c => c.Base));
                    }
                    case EntityKeys.NamespaceKey:
                    {
                        bool prior206Target = CheckProjectTargets();
                        IEnumerable<string> relevantTypes = TemplateEntity.Decorate(owner).EntityHierarchy
                                                                          .Select(CodeEntity.Decorate)
                                                                          .Where(c => !c.IsRoot())
                                                                          .Select(c => c.FullName);
                        string ns = prior206Target
                                        ? codeModel.RootNamespaceForOldTarget(
                                            relevantTypes.ToArray(), GetPortStructures().Concat(GetPortEnums())
                                                                                        .Select(c => c.FullName)
                                                                                        .ToArray())
                                        : codeModel.RootNamespace(relevantTypes.ToArray());
                        if (string.IsNullOrEmpty(ns))
                        {
                            ns = owner.Name;
                        }

                        return owner.Create(key, ns);
                    }
                    default:
                        throw new ContentProviderException(key, owner);
                }

                bool CheckProjectTargets()
                {
                    IEnumerable<TargetEntity> targets = ProjectEntity.Decorate(owner).Targets.Select(t => TargetEntity.Decorate(t));
                    return targets.Any(t => t.Version < new Version(20, 6));
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

            (bool success, string value) FormatIecDataType(string unformattedValue)
            {
                string formattedString = FormatStringDataType(unformattedValue);
                if (formattedString != null)
                {
                    return (true, formattedString);
                }

                (bool success, string value) = FormatDataType(unformattedValue);
                
                if (!success)
                {
                    return (success, value);
                }

                string result = owner.Create("temporaryIecFormatContainer", value).Format()["iecDataType"].Value<string>();
                Match unknownMatch = UnknownDataTypeRegex.Match(result);

                if (unknownMatch.Success)
                {
                    return (false, unknownMatch.Groups["dataType"].Value);
                }

                return (true, result);
            }

            string FormatStringDataType(string unformattedValue)
            {
                Match stringMatch = StaticStringRegex.Match(unformattedValue);
                if(stringMatch.Success)
                {
                    return owner.Create("temporaryStaticStringFormatContainer", unformattedValue)
                                .Format()["iecStringDataType"].Value<string>();
                }
                return null;
            }

            Entity ResolveIECDataType()
            {
                IEntityBase dataSource = ownerTemplateEntity.FormatOrigin;
                IDataType dataSourceDataType = dataSource.HasValue<IDataType>()
                                                   ? dataSource.Value<IDataType>()
                                                   : dataSource.Value<IField>().DataType;

                (bool success, string dataTypeName) = FormatIecDataType(dataSourceDataType.Name);

                if(!success)
                { 
                    ICodeModel rootCodeModel = dataSource.Root.Value<ICodeModel>();
                    IType knownType = dataSourceDataType.PotentialFullNames
                                      .Select(n => rootCodeModel.Type(n))
                                      .FirstOrDefault(t => t != null);
                    if(knownType == null)
                    {
                        throw new UnknownIecDataTypeException(dataTypeName);
                    }
                }
                dataTypeName = dataTypeName.Contains("::")
                    ? dataTypeName.Substring(dataTypeName.LastIndexOf("::", StringComparison.InvariantCulture) + "::".Length)
                    : dataTypeName;

                return owner.Create(key, dataTypeName);   
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

            Entity GetBigDataProjectEntity()
            {
                if (HasMoreThan1000Fields())
                {
                    return owner.Create(key, "true", true);
                }
                return owner.Create(key, "false", false);
            }

            Entity GetNormalProjectEntity()
            {
                if (HasMoreThan1000Fields())
                {
                    return owner.Create(key, "false", false);
                }
                return owner.Create(key, "true", true);
            }
            bool HasMoreThan1000Fields()
            {
                ICodeModel model = owner.Value<ICodeModel>();
                if (model == null ||
                    GetAllPorts().Concat(GetPortStructures().SelectMany(s => s.Fields)).Count() <= 1000)
                    return false;
                return true;
            }
        }
    }
}
