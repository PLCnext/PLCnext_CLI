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
using PlcNext.Common.Project;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools;

namespace PlcNext.Common.CodeModel
{
    internal class PortContentProvider : PriorityContentProvider
    {
        private static readonly Regex StaticStringRegex =
            new(@"^(?:(?:::)?Arp\S*::)?Static(W)?String<(?<length>\w*)>$", RegexOptions.Compiled);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return owner.HasValue<ICodeModel>() &&
                   (key == EntityKeys.PortStructsKey ||
                    key == EntityKeys.PortAndTypeInformationStructs ||
                    key == EntityKeys.PortEnumsKey ||
                    key == EntityKeys.PortAndTypeInformationEnumsKey ||
                    key == EntityKeys.PortArraysKey ||
                    key == EntityKeys.PortAndTypeInformationArraysKey  ||
                    key == EntityKeys.VariablePortStringsKey ||
                    key == EntityKeys.VariablePortAndTypeInformationStringsKey) ||
                   key == EntityKeys.BigDataProjectKey ||
                   key == EntityKeys.NormalProjectKey;
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            ICodeModel codeModel = owner.Value<ICodeModel>();
            switch (key)
            {
                case EntityKeys.BigDataProjectKey:
                    return GetBigDataProjectEntity();
                case EntityKeys.NormalProjectKey:
                    return GetNormalProjectEntity();
                case EntityKeys.PortStructsKey:
                {
                    return owner.Create(key, GetPortStructures(owner).Select(c => c.Base));
                }
                case EntityKeys.PortAndTypeInformationStructs:
                {
                    return owner.Create(key, GetPortAndTypeInformationStructures(owner).Select(c => c.Base));
                }
                case EntityKeys.PortEnumsKey:
                {
                    return owner.Create(key, GetPortEnums().Select(c => c.Base));
                }
                case EntityKeys.PortAndTypeInformationEnumsKey:
                {
                    return owner.Create(key, GetPortAndTypeInformationEnums().Select(c => c.Base));
                }
                case EntityKeys.PortArraysKey:
                {
                    return owner.Create(key, GetPortArrays().Select(c => c.Base));
                }
                case EntityKeys.PortAndTypeInformationArraysKey:
                {
                    return owner.Create(key, GetPortAndTypeInformationArrays().Select(c => c.Base));
                }
                case EntityKeys.VariablePortStringsKey:
                {
                    return owner.Create(key, GetVariablePortStrings().Select(c => c.Base));
                }
                case EntityKeys.VariablePortAndTypeInformationStringsKey:
                {
                    return owner.Create(key, GetVariablePortAndTypeInformationStrings().Select(c => c.Base));
                }
                default:
                    throw new ContentProviderException(key, owner);
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
                    GetAllPorts(owner).Concat(GetPortAndTypeInformationStructures(owner).SelectMany(s => s.Fields))
                                      .Count() <= 1000)
                    return false;
                return true;
            }

            IEnumerable<CodeEntity> GetPortEnums()
            {
                return GetAllPorts(owner).Concat(GetPortStructures(owner).SelectMany(p => p.Fields))
                                         .Select(f => f.ResolvedType)
                                         .Where(t => t.AsEnum != null)
                                         .Distinct(new FullNameCodeEntityComparer());
            }

            IEnumerable<CodeEntity> GetPortAndTypeInformationEnums()
            {
                IEnumerable<CodeEntity> GetAllEnums()
                {
                    ICodeModel model = owner.Root.Value<ICodeModel>();
                    IEnumerable<IEnum> enums = model.Enums.Keys;
                    return enums.Select(s => CodeEntity.Decorate(owner.Create(s?.FullName, s)));
                }

                bool HasTypeInformationAttribute(CodeEntity entity)
                {
                    return entity.AsType != null &&
                           entity.AsType.HasAttributeWithoutValue(EntityKeys.TypeInformationAttributeKey);
                }

                HashSet<CodeEntity> typeInfoAttributedEnums =
                    new(GetAllEnums().Where(HasTypeInformationAttribute),
                        new FullNameCodeEntityComparer());

                return GetAllPorts(owner).Concat(GetPortAndTypeInformationStructures(owner).SelectMany(p => p.Fields))
                                         .Select(f => f.ResolvedType)
                                         .Where(t => t.AsEnum != null)
                                         .Concat(typeInfoAttributedEnums)
                                         .Distinct(new FullNameCodeEntityComparer());
            }

            IEnumerable<CodeEntity> GetPortArrays()
            {
                return GetAllPorts(owner).Concat(GetPortStructures(owner).SelectMany(p => p.Fields))
                                         .Where(IsArray)
                                         .Distinct();
            }


            IEnumerable<CodeEntity> GetPortAndTypeInformationArrays()
            {
                IEnumerable<CodeEntity> typeInfoAttributedArrays = GetAllArrays().Where(HasTypeInformationAttribute);

                return GetAllPorts(owner).Concat(GetPortAndTypeInformationStructures(owner).SelectMany(p => p.Fields))
                                         .Where(IsArray)
                                         .Concat(typeInfoAttributedArrays);

                IEnumerable<CodeEntity> GetAllArrays()
                {
                    return TemplateEntity.Decorate(owner)
                                 .EntityHierarchy.Select(CodeEntity.Decorate)
                                 .SelectMany(c => c.Fields)
                                 .Where(IsArray);
                }

                bool HasTypeInformationAttribute(CodeEntity entity)
                {
                    return entity.AsField != null &&
                           entity.AsField.HasAttributeWithoutValue(EntityKeys.TypeInformationAttributeKey);
                }
            }

            bool IsArray(CodeEntity entity)
            {
                return entity.AsField != null && entity.AsField.Multiplicity.Count > 0;
            }

            IEnumerable<CodeEntity> GetVariablePortStrings()
            {
                // only add fields of hidden structs bec. for non-hidden structs with string field
                // the string datatype will be generated directly in struct definition of dt-worksheet
                return GetAllPorts(owner).Concat(GetPortStructures(owner)
                                                .Where(s => s.IsHidden())
                                                .SelectMany(p => p.Fields))
                                         .Where(IsStringFieldWithNonStandardLegth)
                                         .Distinct();
            }

            bool IsStringFieldWithNonStandardLegth(CodeEntity entity)
            {
                IField field = entity.AsField;
                if (field == null)
                    return false;

                return StaticStringRegex.IsMatch(field.DataType.Name)
                    && StaticStringRegex.Match(field.DataType.Name).Groups["length"].Value != "80"
                    && !string.IsNullOrEmpty(StaticStringRegex.Match(field.DataType.Name)
                                                              .Groups["length"].Value)
                    && field.Multiplicity.Count == 0;
            }

            IEnumerable<CodeEntity> GetVariablePortAndTypeInformationStrings()
            {
                IEnumerable<CodeEntity> typeInfoAttributedStrings = GetAllStrings().Where(HasTypeInformationAttribute);

                return GetAllPorts(owner).Concat(GetPortAndTypeInformationStructures(owner)
                                                .Where(s => s.IsHidden())
                                                .SelectMany(p => p.Fields))
                                         .Where(IsStringFieldWithNonStandardLegth)
                                         .Concat(typeInfoAttributedStrings)
                                         .Distinct();

                IEnumerable<CodeEntity> GetAllStrings()
                {
                    return TemplateEntity.Decorate(owner)
                                 .EntityHierarchy.Select(CodeEntity.Decorate)
                                 .SelectMany(c => c.Fields)
                                 .Where(IsStringFieldWithNonStandardLegth)
                                 .Distinct();
                }

                bool HasTypeInformationAttribute(CodeEntity entity)
                {
                    return entity.AsField != null &&
                           entity.AsField.HasAttributeWithoutValue(EntityKeys.TypeInformationAttributeKey);
                }
            }
        }

        private HashSet<CodeEntity> GetPortAndTypeInformationStructures(Entity owner)
        {
            IEnumerable<CodeEntity> GetAllStructs()
            {
                ICodeModel model = owner.Root.Value<ICodeModel>();
                IEnumerable<IStructure> structures = model.Structures.Keys;
                return structures.Select(s => CodeEntity.Decorate(owner.Create(s?.FullName, s)));
            }

            bool HasTypeInformationAttribute(CodeEntity structEntity)
            {
                return structEntity.AsType != null &&
                       structEntity.AsType.HasAttributeWithoutValue(EntityKeys.TypeInformationAttributeKey);
            }

            HashSet<CodeEntity> typeInfoAttributedStructs =
                new(GetAllStructs().Where(HasTypeInformationAttribute),
                    new FullNameCodeEntityComparer());

            HashSet<CodeEntity> structures =
                new(GetPortStructures(owner).Concat(typeInfoAttributedStructs),
                    new FullNameCodeEntityComparer());


            HashSet<CodeEntity> visited = new(new FullNameCodeEntityComparer());
            while (structures.Except(visited).Any())
            {
                CodeEntity structure = structures.Except(visited).First();
                
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

            return structures;
        }

        private HashSet<CodeEntity> GetPortStructures(Entity owner)
        {
            HashSet<CodeEntity> structures = new(GetAllPorts(owner)
                                                .Select(f => f.ResolvedType)
                                                .Where(t => t.AsType != null && t.AsEnum == null),
                                                 new FullNameCodeEntityComparer());

            HashSet<CodeEntity> visited = new(new FullNameCodeEntityComparer());
            while (structures.Except(visited).Any())
            {
                CodeEntity structure = structures.Except(visited).First();
                
                IEnumerable<string> duplicateFields = structure.Fields.GroupBy(f => f.Name)
                                                               .Where(g => g.Count() > 1)
                                                               .Select(g => g.Key)
                                                               .ToArray();
                if (duplicateFields.Any())
                {
                    throw new PortStructFieldAmbiguousException(structure.FullName, duplicateFields);
                }

                foreach (CodeEntity structureDataType in structure.Fields.Select(f => f.ResolvedType)
                                                                  .Where(t => t.AsType != null && t.AsEnum == null))
                {
                    structures.Add(structureDataType);
                }

                visited.Add(structure);
            }

            return structures;
        }

        private IEnumerable<CodeEntity> GetAllPorts(Entity owner)
        {
            bool IsPort(CodeEntity fieldEntity)
            {
                return fieldEntity.AsField != null &&
                       fieldEntity.AsField.HasAttributeWithoutValue(EntityKeys.PortAttributeKey);
            }

            return TemplateEntity.Decorate(owner)
                                 .EntityHierarchy.Select(CodeEntity.Decorate)
                                 .SelectMany(c => c.Fields)
                                 .Where(IsPort);
        }

        private class FullNameCodeEntityComparer : IEqualityComparer<CodeEntity>
        {
            public bool Equals(CodeEntity x, CodeEntity y)
            {
                return x == null && y == null ||
                       y != null && x != null &&
                       $"{x.Namespace}.{x.Name}" == $"{y.Namespace}.{y.Name}";
            }

            public int GetHashCode(CodeEntity obj)
            {
                return $"{obj.Namespace}.{obj.Name}".GetHashCode(StringComparison.Ordinal);
            }
        }
    }
}