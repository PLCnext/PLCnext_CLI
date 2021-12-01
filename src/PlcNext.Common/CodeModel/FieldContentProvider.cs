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
using PlcNext.Common.DataModel;
using PlcNext.Common.Templates;
using PlcNext.Common.Templates.Field;
using PlcNext.Common.Tools;

namespace PlcNext.Common.CodeModel
{
    internal class FieldContentProvider : PriorityContentProvider
    {
        private readonly ITemplateRepository templateRepository;
        private readonly ITemplateResolver resolver;

        public FieldContentProvider(ITemplateRepository templateRepository, ITemplateResolver resolver)
        {
            this.templateRepository = templateRepository;
            this.resolver = resolver;
        }

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            if (!owner.HasValue<IField>())
            {
                return false;
            }
            
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

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            IField field = owner.Value<IField>();
            switch (key)
            {
                case EntityKeys.FieldNameKey:
                {
                    return owner.Create(key, field.Name);
                }
                case EntityKeys.DataTypeKey:
                {
                    IDataType fieldDataType = field.DataType;
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
                    return owner.Create(
                        key, field.Multiplicity.Select(m => owner.Create(key, new Func<string>(m.ToString), m)));
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
                        return GetFieldTemplateEntity(metaDataTemplate, field, owner, key);
                    }

                    throw new ContentProviderException(key, owner);
                }
            }
        }

        private Entity GetFieldTemplateEntity(metaDataTemplate metaDataTemplate, IField field, Entity owner, string key)
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
                        owner.AddCodeException(
                            new FieldAttributeRestrictionException(metaDataTemplate.name, arg, message, position,
                                                                   field.GetFile(owner)));
                    }

                    return newValue;
                }
            }

            (string[], CodePosition) GetFieldTemplateValue()
            {
                IAttribute attribute =
                    field.Attributes.LastOrDefault(
                        a => a.Name.Equals(metaDataTemplate.name, StringComparison.OrdinalIgnoreCase));
                string value = string.IsNullOrEmpty(attribute?.Values.FirstOrDefault())
                                   ? resolver.Resolve(metaDataTemplate.defaultvalue, owner)
                                   : attribute.Values.First();
                return (metaDataTemplate.multiplicity == multiplicity.OneOrMore
                            ? value.Split(new[] { metaDataTemplate.split }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(v => v.Trim()).ToArray()
                            : new[] { value }, attribute?.Position ?? new CodePosition(0, 0));
            }
        }
    }
}